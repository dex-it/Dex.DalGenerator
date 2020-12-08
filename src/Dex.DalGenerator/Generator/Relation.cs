namespace Dex.DalGenerator.Generator
{
    public class Relation
    {
        public string EntityName { get; set; }
        public string TypeName { get; set; }
        public bool IsCollection { get; set; }
        public string KeyPropertyName { get; set; }
        public string PropertyName { get; set; }
        public bool OneToOne { get; set; }
        public bool IsBackRelation { get; set; }
        public bool IsCascadeDelete { get; set; }
    }
}