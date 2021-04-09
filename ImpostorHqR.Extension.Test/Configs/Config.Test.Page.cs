using ImpostorHq.Extension.Api.Interface;

namespace ImpostorHq.Extension.Test.Configs
{
    public class TestPageConfig : IConfigurationHolder
    {
        public static TestPageConfig Instance;

        public int UpdateInterval { get; set; }

        public TestPageConfig()
        {
            Instance = this;
        }

        public void SetDefaults()
        {
            this.UpdateInterval = 1000;
        }
    }
}
