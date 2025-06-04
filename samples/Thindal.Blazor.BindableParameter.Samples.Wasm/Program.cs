using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Thindal.Blazor.BindableParameter.Samples.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();


/*namespace Thindal.Blazor.BindableParameter.Samples.Wasm.Components
{
    public partial class Testcomponent
    {
        private int _count;
        [Parameter] public EventCallback<int> OnCountChanged { get; set; }

        [Parameter]
        public partial int Count
        {
            get => _count;
            set
            {
                if (_count == value)
                {
                    return;
                }

                _count = value;
                OnCountChanged.InvokeAsync(value);
            }
        }
    }
}*/