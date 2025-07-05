namespace neo.admin.Models
{
    public sealed record FaskesInfoResponse(
    long Id,
    string Name,
    string Email,
    string Phone,
    string Address,
    bool IsActive,
    long? CorporateId,
    string? CorporateName);
}
