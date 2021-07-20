using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Dex.DalGenerator.Core.Extensions;

namespace Dex.DalGenerator.Core.Helpers
{
    /// <summary>
    ///     A base type for conventions that configure model aspects based on whether the member type
    ///     is a non-nullable reference type.
    /// </summary>
    public static class NullableRefConvention
    {
        // The [Nullable] attribute is synthesized by the compiler. It's best to just compare the type name.
        private const string NullableAttributeFullName = "System.Runtime.CompilerServices.NullableAttribute";
        private const string NullableFlagsFieldName = "NullableFlags";

        private const string NullableContextAttributeFullName = "System.Runtime.CompilerServices.NullableContextAttribute";
        //private const string NullableContextFlagsFieldName = "Flag";

        // 2 means "nullable". 1 means "not nullable", and 0 means "oblivious".
        //private const byte ObliviousFlag = 0;
        private const byte NotNullableFlag = 1;
        //private const byte NullableFlag = 2;

        public static bool IsNonNullableReferenceType(ParameterInfo parameterInfo)
        {
            Debug.Assert(parameterInfo != null);

            return IsNonNullableReferenceType(parameterInfo, parameterInfo.ParameterType, parameterInfo.Member.DeclaringType);
        }

        public static bool IsNonNullableReferenceType(Type memberType)
        {
            Debug.Assert(memberType != null);

            return IsNonNullableReferenceType(memberType, memberType, memberType.DeclaringType);
        }

        public static bool IsNonNullableReferenceType(MemberInfo memberInfo)
        {
            Debug.Assert(memberInfo != null);

            return IsNonNullableReferenceType(memberInfo, memberInfo.GetMemberType(), memberInfo.DeclaringType);
        }

        private static bool IsNonNullableReferenceType(ICustomAttributeProvider memberInfo, Type memberType, Type? declaringType)
        {
            Debug.Assert(memberInfo != null);

            if (memberType.IsValueType == true)
                return false;

            // First check for [MaybeNull] on the return value. If it exists, the member is nullable.
            // Note: avoid using GetCustomAttribute<> below because of https://github.com/mono/mono/issues/17477
            bool isMaybeNull = memberInfo switch
            {
                FieldInfo f
                    => f.CustomAttributes.Any(a => a.AttributeType == typeof(MaybeNullAttribute)),
                ParameterInfo pi
                    => pi.CustomAttributes.Any(a => a.AttributeType == typeof(MaybeNullAttribute)),
                PropertyInfo p
                    => p.GetMethod?.ReturnParameter?.CustomAttributes?.Any(a => a.AttributeType == typeof(MaybeNullAttribute)) == true,
                _ => false
            };

            if (isMaybeNull)
            {
                return false;
            }

            // For C# 8.0 nullable types, the C# compiler currently synthesizes a NullableAttribute that expresses nullability into
            // assemblies it produces. If the model is spread across more than one assembly, there will be multiple versions of this
            // attribute, so look for it by name, caching to avoid reflection on every check.
            // Note that this may change - if https://github.com/dotnet/corefx/issues/36222 is done we can remove all of this.

            // First look for NullableAttribute on the member itself.
            // The [Nullable] and [NullableContext] attributes are not inherited.
            if (TryIsNonNullableMember(memberInfo, out bool isNonNullable))
            {
                return isNonNullable;
            }

            // No attribute on the member, try to find a NullableContextAttribute on the declaring type.
            // The [NullableContext] attribute can appear on a method or on the module.
            return IsNonNullableBasedOnContext(declaringType, memberType.Module);
        }

        private static bool TryIsNonNullableMember(ICustomAttributeProvider memberInfo, out bool isNonNullable)
        {
            if (memberInfo.GetCustomAttributes(inherit: false).FirstOrDefault(a => a.GetType().FullName == NullableAttributeFullName)
                is Attribute attribute)
            {
                // We don't handle cases where generics and NNRT are used. This runs into a
                // fundamental limitation of ModelMetadata - we use a single Type and Property/Parameter
                // to look up the metadata. However when generics are involved and NNRT is in use
                // the distance between the [Nullable] and member we're looking at is potentially
                // unbounded.
                //
                // See: https://github.com/dotnet/roslyn/blob/master/docs/features/nullable-reference-types.md#annotations
                if (attribute.GetType().GetField(NullableFlagsFieldName) is FieldInfo field &&
                    field.GetValue(attribute) is byte[] flags)
                {
                    // First element is the property/parameter type.
                    isNonNullable = flags.FirstOrDefault() == NotNullableFlag;
                    return true; // Найден [Nullable]
                }
            }
            isNonNullable = default;
            return false; // Не найден [Nullable]
        }

        private static bool IsNonNullableBasedOnContext(Type? declaringType, Module module)
        {
            // Check on the containing type.
            //Type? declaringType = memberInfo.DeclaringType;
            if (declaringType != null)
            {
                // For generic types, inspecting the nullability requirement additionally requires
                // inspecting the nullability constraint on generic type parameters. This is fairly non-triviial
                // so we'll just avoid calculating it. Users should still be able to apply an explicit [Required]
                // attribute on these members.
                if (declaringType.IsGenericType == true)
                {
                    return false; // Может быть Null по умолчанию.
                }

                do
                {
                    Attribute[] attributes = Attribute.GetCustomAttributes(declaringType, inherit: false);
                    if (TryFindNullableContext(attributes, out bool isNonNullable))
                    {
                        return isNonNullable;
                    }

                    declaringType = declaringType.DeclaringType;
                } while (declaringType != null);
            }
            else
            {
                //return false; // Может быть Null по умолчанию.
            }

            // If we don't find the attribute on the declaring type then repeat at the module level.
            return IsNonNullableBasedOnModule(module);
        }

        private static bool IsNonNullableBasedOnModule(Module module)
        {
            var attributes = Attribute.GetCustomAttributes(module, inherit: false);
            return TryFindNullableContext(attributes, out bool isNonNullable) && isNonNullable;
        }

        private static bool TryFindNullableContext(Attribute[] attributes, out bool isNonNullable)
        {
            if (attributes.FirstOrDefault(a => string.Equals(a.GetType().FullName, NullableContextAttributeFullName, StringComparison.Ordinal))
                is Attribute contextAttr)
            {
                if (contextAttr.GetType().GetField("Flag") is FieldInfo field && field.GetValue(contextAttr) is byte flag)
                {
                    isNonNullable = flag == NotNullableFlag;
                    return true; // Нашли [NullableContext]
                }
            }
            isNonNullable = default;
            return false;
        }

        //private sealed class NonNullabilityConventionState
        //{
        //    public Type NullableAttrType;
        //    public Type NullableContextAttrType;
        //    public FieldInfo NullableFlagsFieldInfo;
        //    public FieldInfo NullableContextFlagFieldInfo;
        //    public Dictionary<Type, bool> TypeCache { get; } = new();
        //}
    }
}
