using Microsoft.AspNetCore.Mvc.Razor;

namespace Infrastructure.Services.Views;

public class InfrastructureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    { }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        var customLocations = new[]
        {
            "wwwroot/Views/Emails/{0}.cshtml",
        };

        return viewLocations.Concat(customLocations);
    }
}