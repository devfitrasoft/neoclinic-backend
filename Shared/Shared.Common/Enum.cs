namespace Shared.Common
{
    public enum OtpType
    {
        PreRegist = 1,
        ResetPwd = 2
    }

    public enum PICCType
    {
        PJ = 1, // general manager/director
        Billing = 2,
        Tech = 3
    }

    public enum BillingType
    {
        Monthly = 1,
        Registration = 2
    }

    public enum MailingStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Retrying = 3
    }
}
