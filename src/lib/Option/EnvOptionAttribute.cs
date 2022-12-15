namespace Swan.Lib.Option
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EnvOptionAttribute : Attribute
    {
        public EnvOptionAttribute(string envName)
        {
            EnvName = envName;
        }

        public string EnvName { get; init; }

        public string Default { get; set; }
    }
}
