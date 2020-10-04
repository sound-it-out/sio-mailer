using System.Threading.Tasks;

namespace SIO.Domain.Emails.Builders
{
    public interface IRazorViewBuilder
    {
        Task<string> BuildAsync(string template, object model);
    }
}
