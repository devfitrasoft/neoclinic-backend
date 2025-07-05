namespace neo.admin.Models
{
    public class RegistrationSettings
    {
        public decimal Fee { get; init; }
        public string BankAccountNumber { get; init; } = string.Empty;
        public string ConfirmPaymentPhoneNumber { get; init; } = string.Empty;
    }
}
