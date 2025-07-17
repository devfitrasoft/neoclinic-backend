namespace neo.admin.Models
{
    public sealed record FaskesInfoResponse(
    long Id,
    string Name,
    string Email,
    string Phone,
    string? EmailBill,
    string? PhoneBill,
    string? EmailTech,
    string? PhoneTech,
    string Address,
    bool IsActive,
    long? CorporateId,
    string? CorporateName);
}
