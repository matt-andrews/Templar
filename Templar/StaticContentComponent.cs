namespace Templar
{
    internal class StaticContentComponent : TemplarComponent
    {
        private readonly string _templatePath;
        public override string TemplatePath => _templatePath;
        public StaticContentComponent(string templatePath)
        {
            _templatePath = templatePath;
        }
        public string GetStaticFile(IStaticSiteService staticSiteService)
        {
            return base.GetFile(staticSiteService);
        }
    }
}
