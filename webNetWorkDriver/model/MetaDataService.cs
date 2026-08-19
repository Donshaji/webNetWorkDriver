using CommunityToolkit.Mvvm.ComponentModel;

namespace webNetWorkDriver.model
{
    public partial class MetaDataService : ObservableObject
    {
        [ObservableProperty]
        private string metaDataContent = "/* No metadata yet */";
    }
}
