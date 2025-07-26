using Shared.Models;

namespace neo.admin.Models
{
    public sealed class CorporateInfoResponse : CommonAPIBodyResponse
    {
        public IEnumerable<CorporateLookupItem> Data { get; set; } = new List<CorporateLookupItem>();
    };
}
