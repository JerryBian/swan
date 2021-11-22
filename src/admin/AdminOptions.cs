using Laobian.Share.Option;

namespace Laobian.Admin;

public class AdminOptions : SharedOptions
{
    [OptionEnvName("ADMIN_PASSWORD")] public string AdminPassword { get; set; }
}