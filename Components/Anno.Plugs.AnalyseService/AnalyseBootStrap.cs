using System;

namespace Anno.Plugs.AnalyseService
{
    using Anno.EngineData;
    [DependsOn(
                typeof(ViperLog.ViperLogBootstrap)
        )]
    public class AnalyseBootStrap : IPlugsConfigurationBootstrap
    {
        public void ConfigurationBootstrap()
        {
           
        }

        public void PreConfigurationBootstrap()
        {
            
        }
    }
}
