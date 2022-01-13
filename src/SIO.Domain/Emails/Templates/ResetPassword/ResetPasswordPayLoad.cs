namespace SIO.Domain.Emails.Templates.ResetPassword
{
    public sealed record ResetPasswordPayLoad(string Email, string Name, string ActivationToken, UrlOptions UrlOptions) : BasePayload(UrlOptions);
}
