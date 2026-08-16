// Models/CssService.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace webNetWorkDriver.model
{
    public partial class CssService : ObservableObject
    {
        [ObservableProperty]
        private string cssContent = "/* No CSS yet */";
    }
}