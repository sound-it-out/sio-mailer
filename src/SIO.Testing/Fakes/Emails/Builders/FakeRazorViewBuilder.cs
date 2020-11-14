using System.Threading.Tasks;
using SIO.Domain.Emails.Builders;

namespace SIO.Testing.Fakes.Notifications.Builders
{
    public sealed class FakeRazorViewBuilder : IRazorViewBuilder
    {
        private readonly string _result;

        public FakeRazorViewBuilder(string result)
        {
            _result = result;
        }

        public Task<string> BuildAsync(string template, object model)
        {
            return Task.FromResult(_result);
        }
    }
}
