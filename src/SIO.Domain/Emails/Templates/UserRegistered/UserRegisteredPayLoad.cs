namespace SIO.Domain.Emails.Templates.UserRegistered
{
    public sealed record UserRegisteredPayLoad(string Email, string Name, string ActivationToken, UrlOptions UrlOptions) : BasePayload(UrlOptions);
}
