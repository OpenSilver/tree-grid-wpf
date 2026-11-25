using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using OpenSilver.WebAssembly;

namespace TreeGrid.Demo.OpenSilver.Browser.Pages
{
    [Route("/")]
    public sealed class Index : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Runner.RunApplicationAsync<TreeGrid.Demo.App>();
        }
    }
}
