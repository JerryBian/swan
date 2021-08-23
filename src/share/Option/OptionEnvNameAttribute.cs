using System;

namespace Laobian.Share.Option
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionEnvNameAttribute : Attribute
    {
        public OptionEnvNameAttribute(string envName)
        {
            EnvName = envName;
        }

        public string EnvName { get; init; }
    }
}