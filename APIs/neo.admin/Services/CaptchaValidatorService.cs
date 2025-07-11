namespace neo.admin.Services
{
    public sealed class CaptchaValidatorService : ICaptchaValidator
    {
        private readonly IConfiguration _cfg;
        private readonly HttpClient _http;
        public CaptchaValidatorService(IConfiguration cfg, IHttpClientFactory f) =>
            (_cfg, _http) = (cfg, f.CreateClient());

        public async Task<bool> VerifyAsync(string token, CancellationToken ct = default)
        {
            if (_cfg.GetValue<bool>("Captcha:Disable"))
                return true;

            var secret = _cfg["Captcha:Secret"];
            var verifyUrl = _cfg["Captcha:VerifyUrl"];
            var resp = await _http.PostAsync(
                $"{verifyUrl}?secret={secret}&response={token}",
                null, ct);

            if (!resp.IsSuccessStatusCode) return false;

            var json = await resp.Content.ReadFromJsonAsync<RecaptchaResp>(cancellationToken: ct);
            return json?.Success == true && json.Score >= 0.5;
        }
        private sealed record RecaptchaResp(bool Success, float Score);
    }
}
