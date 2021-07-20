using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Dex.DalGenerator.Tests
{
#nullable disable
    public class DefaultClassModel
    {
        public string Name { get; set; }
    }
#nullable restore

    public class TestMe
    {
        [MaybeNull] public string Name1 { get; set; }
        [AllowNull] public string Name2 { get; set; }

        public string? Name3;
        public string Name4;
        [MaybeNull] public string Name5;
        [AllowNull] public string Name6;
    }

    public class UserModel<T>
    {
        public string Name { get; set; }
        public T Surname { get; set; }
    }

    public record UserModel(string Name, string? Surname);
}

namespace InternalNUnitTest
{
    using Dex.DalGenerator.Core.Helpers;
    using Dex.DalGenerator.Tests;

    public class NullableTests
    {
        [Fact]
        public void NotNull_Property()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(UserModel).GetProperty(nameof(UserModel.Name))!);

            Assert.True(isNonNull, "��������� �������� �� ��������� Null �� ������ ���������");
        }

        [Fact]
        public void CanBeNull_ByDefault()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(DefaultClassModel).GetProperty(nameof(DefaultClassModel.Name))!);

            Assert.False(isNonNull, "��������� �������� ��������� Null �� ���������");
        }

        [Fact]
        public void CanBeNull_Property()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(UserModel).GetProperty(nameof(UserModel.Surname))!);

            Assert.False(isNonNull, "��������� �������� ���� ��������� Null (���� '?')");
        }

        [Fact]
        public void NotNull_Generic()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
               memberInfo: typeof(UserModel<string>).GetProperty(nameof(UserModel<string>.Surname))!);

            Assert.False(isNonNull, "��������� �������� ��������� Null �� ��������� ��� Generic �������.");
        }

        [Fact]
        public void CanBeNull_Generic()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(UserModel<string?>).GetProperty(nameof(UserModel<string?>.Surname))!);

            Assert.False(isNonNull, "��������� �������� ��������� Null �� ��������� ��� Generic �������.");
        }

        [Fact]
        public void MaybeNull_Attribute_OnProperty()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(TestMe).GetProperty(nameof(TestMe.Name1))!);

            Assert.False(isNonNull, "��������� �������� �� ��������� Null �� ������ ���������, �� ���� ����������� �������");
        }

        [Fact]
        public void AllowNull_Attribute_OnProperty()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(TestMe).GetProperty(nameof(TestMe.Name2))!);

            // TODO ��������� ��� AllowNull �� ������ ��������� ��������� Null.
            Assert.True(isNonNull, "��������� �������� �� ��������� Null �� ������ ���������, �� ������ �� ������� AllowNull");
        }

        [Fact]
        public void Nullable_Field()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(TestMe).GetField(nameof(TestMe.Name3))!);

            Assert.False(isNonNull, "��������� ���� ���� ��������� Null (���� '?')");
        }

        [Fact]
        public void NotNull_Field()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(TestMe).GetField(nameof(TestMe.Name4))!);

            Assert.True(isNonNull, "��������� ���� �� ��������� Null �� ������ ���������");
        }

        [Fact]
        public void MaybeNull_Attribute_OnField()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(TestMe).GetField(nameof(TestMe.Name5))!);

            Assert.False(isNonNull, "��������� ���� �� ��������� Null �� ������ ���������, �� ���� ����������� �������");
        }

        [Fact]
        public void AllowNull_Attribute_OnField()
        {
            bool isNonNull = NullableRefConvention.IsNonNullableReferenceType(
                memberInfo: typeof(TestMe).GetField(nameof(TestMe.Name6))!);

            // TODO ��������� ��� AllowNull �� ������ ��������� ��������� Null.
            Assert.True(isNonNull, "��������� ���� �� ��������� Null �� ������ ���������, �� ������ �� ������� AllowNull");
        }
    }
}
