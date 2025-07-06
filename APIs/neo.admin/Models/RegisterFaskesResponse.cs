namespace neo.admin.Models
{
    public sealed record RegisterFaskesResponse(
        bool isRegistered,
        bool preExisted
    );
}
