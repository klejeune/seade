using System;
using System.Collections.Generic;
using Seade.Core;

namespace Seade.Test.Models
{
    public class ConfigLevel1 : ConfigurationBase
    {
        public ConfigLevel1(IConfigurationService configurationService) : base(configurationService) {}

        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int Integer { get; set; }
        public double Double { get; set; }
        public string[] StringArray { get; set; }
        public ConfigLevel2 Object { get; set; }
        public List<ConfigLevel2> ObjectList { get; set; }
        public int[] Array { get; set; }
        public IEnumerable<string> EnumerableList { get; set; }
        public string ReadOnlyText { get; private set; }
        public string IgnoredText { get; }
    }
}