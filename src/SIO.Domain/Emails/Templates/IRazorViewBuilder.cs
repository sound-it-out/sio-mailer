using SIO.Infrastructure.Events;

namespace SIO.Domain.Emails.Templates
{
    public interface IRazorViewBuilder
    {
        Task<string> BuildAsync(string template, object @event);
    }
}
