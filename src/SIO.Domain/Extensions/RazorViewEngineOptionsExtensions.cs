using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.FileProviders;

namespace SIO.Domain.Extensions
{
    public static class RazorViewEngineOptionsExtensions
    {
        public static MvcRazorRuntimeCompilationOptions AddEmailTemplates(this MvcRazorRuntimeCompilationOptions options)
        {
            options.FileProviders.Add(new EmbeddedFileProvider(
                typeof(RazorViewEngineOptionsExtensions).GetTypeInfo().Assembly,
                "SIO.Domain"
            ));

            return options;
        }
    }
}
