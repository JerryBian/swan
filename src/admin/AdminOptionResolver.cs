using Laobian.Share.Option;
using Microsoft.Extensions.Configuration;

namespace Laobian.Admin
{
    public class AdminOptionResolver : CommonOptionResolver
    {
        public void Resolve(AdminOption option, IConfiguration configuration)
        {
            base.Resolve(option, configuration);
            option.AdminPassword = configuration.GetValue<string>("ADMIN_PASSWORD");
        }
    }
}