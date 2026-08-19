using CommunityToolkit.Mvvm.ComponentModel;
using webNetWorkDriver.model;

namespace webNetWorkDriver.ViewModel
{
    public partial class MetaDataViewerViewModel : ObservableObject
    {
        private readonly MetaDataService _metaDataService;

        public string MetaDataContent => _metaDataService.MetaDataContent;

        public MetaDataViewerViewModel(MetaDataService metaDataService)
        {
            _metaDataService = metaDataService;
            // Subscribe to changes in the service
            _metaDataService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MetaDataService.MetaDataContent))
                {
                    OnPropertyChanged(nameof(MetaDataContent));
                }
            };
        }
    }
}
