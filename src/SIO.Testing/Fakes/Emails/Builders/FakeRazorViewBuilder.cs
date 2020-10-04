using System.Threading.Tasks;
using SIO.Domain.Emails.Builders;

namespace SIO.Testing.Fakes.Notifications.Builders
{
    public sealed class FakeRazorViewBuilder : IRazorViewBuilder
    {
        public Task<string> BuildAsync(string template, object model)
        {
            return Task.FromResult(template);
        }
    }
}
