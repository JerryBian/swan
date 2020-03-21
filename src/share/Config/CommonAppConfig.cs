namespace Laobian.Share.Config
{
    public class CommonAppConfig : IConfig
    {
        [ConfigMeta(Name = "SEND_GRID_API_KEY", Required = true)]
        public string SendGridApiKey { get; set; }

        [ConfigMeta(Name = "ADMIN_USER_NAME", Required = true)]
        public string AdminUserName { get; set; }

        [ConfigMeta(Name = "ADMIN_PASSWORD", Required = true)]
        public string AdminPassword { get; set; }

        [ConfigMeta(Name = "MARKDOWN_EXTENSION", DefaultValue = ".md")]
        public string MarkdownExtension { get; set; }

        [ConfigMeta(Name = "HTML_EXTENSION", DefaultValue = ".html")]
        public string HtmlExtension { get; set; }

        [ConfigMeta(Name = "ADMIN_CHINESE_NAME", DefaultValue = "卞良忠")]
        public string AdminChineseName { get; set; }

        [ConfigMeta(Name = "ADMIN_ENGLISH_NAME", DefaultValue = "Jerry Bian")]
        public string AdminEnglishName { get; set; }

        [ConfigMeta(Name = "ADMIN_EMAIL", DefaultValue = "JerryBian@outlook.com")]
        public string AdminEmail { get; set; }

        [ConfigMeta(Name = "ALERT_SENDER_NAME", DefaultValue = "Alert Cat")]
        public string AlertSenderName { get; set; }

        [ConfigMeta(Name = "ALERT_SENDER_EMAIL", DefaultValue = "alert-cat@laobian.me")]
        public string AlertSenderEmail { get; set; }
    }
}