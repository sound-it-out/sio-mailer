using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
