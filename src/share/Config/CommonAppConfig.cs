namespace Laobian.Share.Config
{
    public class CommonAppConfig : IConfig
    {
        [ConfigMeta(Name = "SEND_GRID_API_KEY", Required = true)]
        public string SendGridApiKey { get; set; }

        [ConfigMeta(Name = "ERROR_LOG_SEND_INTERVAL", DefaultValue = 60)]
        public int ErrorLogsSendInterval { get; set; }

        [ConfigMeta(Name = "WARNING_LOGS_SEND_INTERVAL", DefaultValue = 300)]
        public int WarningLogsSendInterval { get; set; }

        [ConfigMeta(Name = "ADMIN_USER_NAME", Required = true)]
        public string AdminUserName { get; set; }

        [ConfigMeta(Name = "ADMIN_PASSWORD", Required = true)]
        public string AdminPassword { get; set; }
    }
}
