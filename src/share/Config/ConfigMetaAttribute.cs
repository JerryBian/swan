using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Config
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigMetaAttribute : Attribute
    {
        public bool Required { get; set; }

        public string Name { get; set; }

        public object DefaultValue { get; set; }
    }
}
