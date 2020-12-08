using System;

namespace Dex.DalGenerator.Annotation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ReferencesAttribute : Attribute
    {
        public Type Type { get; }
        public bool Cascade { get; set; }
        public bool OneToOne { get; set; }
        public string DisplayField { get; set; }
    
        public ReferencesAttribute(Type type)
        {
            Type = type;
        }
    }
}