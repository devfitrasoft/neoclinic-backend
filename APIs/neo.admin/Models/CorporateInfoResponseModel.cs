using Shared.Models;

namespace neo.admin.Models
{
    public sealed class CorporateInfoResponseModel : CommonAPIBodyResponseModel
    {
        public IEnumerable<CorporateLookupItemModel> Data { get; set; } = new List<CorporateLookupItemModel>();
    };
}
