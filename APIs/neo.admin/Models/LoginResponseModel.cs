using Shared.Entities.Objs.Enterprise;
using Shared.Models;

namespace neo.admin.Models
{
    public sealed class LoginResponseModel : CommonAPIBodyResponseModel
    {
        public SessionDataModel Data { get; set; } = null!;
    }
}
