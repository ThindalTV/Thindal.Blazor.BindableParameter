namespace Thindal.Blazor.BindableParameter.Samples.Server.Components;
using Thindal.Blazor.BindableParameter;
public partial class InnerCounter
{
    [BindableParameter] public partial int Count { get; set; }

    [BindableParameter(nameof(Count))] public partial int CountAlsoCallingCountChanged { get; set; }

    [BindableParameter(false, nameof(Count))] public partial int CountOnlyCallingCountChanged { get; set; }
}
