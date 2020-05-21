using Nancy;
using Nancy.Conventions;

namespace HondaGen
{
    public class ApplicationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("views", "views"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("wwwroot", "wwwroot"));
            base.ConfigureConventions(nancyConventions);
        }
    }
}
