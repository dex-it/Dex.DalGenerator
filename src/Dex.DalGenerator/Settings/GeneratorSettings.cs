namespace Dex.DalGenerator.Settings
{
    public class GeneratorSettings
    {
        public string Root { get; set; }
        
        public string CsProject { get; set; }
        
        public string Dll { get; set; }

        public GenElement DbModels { get; set; }
        
        public GenElement DbFluentFk { get; set; }
        
        public GenElement DbFluentEnum { get; set; }
        
        public GenElement Dto { get; set; }
    }
}