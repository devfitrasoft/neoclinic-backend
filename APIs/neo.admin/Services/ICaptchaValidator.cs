namespace neo.admin.Services
{
    public interface ICaptchaValidator
    {
        Task<bool> VerifyAsync(string token, CancellationToken ct = default);
    }
}
