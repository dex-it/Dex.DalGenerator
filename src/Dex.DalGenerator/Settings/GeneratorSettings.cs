namespace Dex.DalGenerator.Settings
{
    public class GeneratorSettings
    {
        public string Root { get; set; }
        
        public string CsProject { get; set; }
        
        public string Dll { get; set; }

        public DbEntityGenElement DbModels { get; set; }
        
        public GenElement DbFluentFk { get; set; }
        
        public GenElement DbFluentIndex { get; set; }
        
        public GenElement DbFluentEnum { get; set; }
        
        public GenElement Dto { get; set; }
    }
}