using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;

namespace SamSafeCSharp.Components
{
    public sealed class TemplateRenderingService
    {
        private static TemplateRenderingService _handleBarRenderer;
        private static IHostingEnvironment _hostingEnvironment;

        private TemplateRenderingService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public static void Init(IHostingEnvironment hostingEnvironment)
        {
            _handleBarRenderer = new TemplateRenderingService(hostingEnvironment);
        }

        public static TemplateRenderingService Instance => _handleBarRenderer;

        internal string RenderHbs(string templateSource, dynamic model)
        {
            var file = MapPath($@"templates/{templateSource}.hbs");
            var template = Handlebars.Compile(File.ReadAllText(file));
            var retval = template(model);
            return retval;
        }

        private static string MapPath(string path)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, path);
            return filePath;
        }
    }
}
