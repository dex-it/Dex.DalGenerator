using System;

namespace Dex.DalGenerator.Annotation.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IdentityAttribute : Attribute
    {
    }
}