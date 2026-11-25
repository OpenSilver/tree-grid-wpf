using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;

namespace TreeGrid.Demo.OpenSilver.Browser
{
    /// <summary>Blazor root component: routes the page to <see cref="Pages.Index"/>.</summary>
    public sealed class App : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<Router>(0);
            builder.AddAttribute(1, nameof(Router.AppAssembly), typeof(Program).Assembly);
            builder.AddAttribute(2, nameof(Router.Found), (RenderFragment<RouteData>)(routeData => found =>
            {
                found.OpenComponent<RouteView>(3);
                found.AddAttribute(4, nameof(RouteView.RouteData), routeData);
                found.CloseComponent();
            }));
            builder.CloseComponent();
        }
    }
}
