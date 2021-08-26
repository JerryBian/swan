using Laobian.Share.Option;

namespace Laobian.Admin
{
    public class LaobianAdminOption : LaobianSharedOption
    {
        [OptionEnvName("ADMIN_PASSWORD")] public string AdminPassword { get; set; }
    }
}