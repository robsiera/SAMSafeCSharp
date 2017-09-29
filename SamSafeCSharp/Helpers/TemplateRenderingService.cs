using System.IO;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Hosting;

namespace SamSafeCSharp.Helpers
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

        internal void RegisterPartial(string key, string templateName)
        {
            var templateSource = ReadTemplate(templateName);
            Handlebars.RegisterTemplate(key, templateSource);
        }

        internal string RenderHbs(string templateName, dynamic model)
        {
            var templateSource = ReadTemplate(templateName);
            var template = Handlebars.Compile(templateSource);
            var retval = template(model);
            return retval;
        }

        private static string ReadTemplate(string templateName)
        {
            var file = MapPath($@"templates/{templateName}.hbs");
            return File.ReadAllText(file);
        }

        private static string MapPath(string path)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, path);
            return filePath;
        }
    }
}
