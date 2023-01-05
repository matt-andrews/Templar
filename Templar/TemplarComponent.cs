using Microsoft.AspNetCore.Http;
using MimeTypes;
using Templar.Attributes;
using Templar.Containers;

namespace Templar
{
    public abstract class TemplarComponent
    {
        public abstract string TemplatePath { get; }
        protected HttpRequest Request { get; private set; } = default!;
        private string? _mimeType;
        public string MimeType
        {
            get
            {
                if (_mimeType is not null)
                    return _mimeType;
                var fileInfo = new FileInfo(TemplatePath);
                return _mimeType = MimeTypeMap.GetMimeType(fileInfo.Extension);
            }
        }
        protected virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }
        protected virtual void SetParameters(IParameters parameters)
        {
            var properties = this.GetType().GetProperties()
                .Where(prop => prop.IsDefined(typeof(ParameterAttribute), false));
            foreach (var prop in properties)
            {
                prop.SetValue(this, parameters[prop.Name.ToLower(), prop.PropertyType]);
            }
            var cascadingProperties = this.GetType().GetProperties()
                .Where(prop => prop.IsDefined(typeof(CascadingParameterAttribute), false));
            foreach (var prop in cascadingProperties)
            {
                prop.SetValue(this, parameters[prop.Name.ToLower(), prop.PropertyType, true]);
            }
        }
        protected virtual string GetFile(IStaticSiteService siteService)
        {
            var result = siteService.GetFile(TemplatePath);
            return result;
        }
        protected virtual void Inject(IServiceProvider serviceProvider)
        {
            var injectProperties = this.GetType().GetProperties()
                .Where(prop => prop.IsDefined(typeof(InjectAttribute), false));
            foreach (var prop in injectProperties)
            {
                prop.SetValue(this, serviceProvider.GetService(prop.PropertyType));
            }
        }
        protected internal static async Task<string> BuildComponents(
            string file,
            IComponents components,
            IStaticSiteService siteService,
            IServiceProvider serviceProvider,
            IParameters parameters,
            HttpRequest req)
        {
            var builder = new ComponentBuilder(file, components, parameters);
            foreach (var result in builder.GetComponents())
            {
                file = await result.Component.Activate()
                    .Initialize(file, result.Tag, components, siteService, serviceProvider, parameters, req);
            }
            return file;
        }
        protected internal async Task<string> Initialize(
            string file,
            string tag,
            IComponents components,
            IStaticSiteService siteService,
            IServiceProvider serviceProvider,
            IParameters parameters,
            HttpRequest req)
        {
            Request = req;
            Inject(serviceProvider);
            SetParameters(parameters);
            await OnInitializedAsync();
            var compile = GetFile(siteService);
            file = file.Replace(tag, compile);
            file = new BindingsBuilder(this, parameters).Build(file);
            return await BuildComponents(file, components, siteService, serviceProvider, parameters, req);
        }
    }
}
