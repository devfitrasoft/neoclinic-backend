using Shared.Entities.Objs.Enterprise;

namespace neo.admin.Models
{
    public sealed class SessionDataModel
    {
        public TokenResultModel Tokens { get; set; } = null!;
        public Login Login { get; set; } = null!;
    }
}
