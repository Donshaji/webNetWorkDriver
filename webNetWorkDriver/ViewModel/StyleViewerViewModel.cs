using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using webNetWorkDriver.model;

namespace webNetWorkDriver.ViewModel
{
    public partial class StyleViewerViewModel : ObservableObject
    {
        private readonly CssService _cssService;

        public string CssContent => _cssService.CssContent;

        public StyleViewerViewModel(CssService cssService)
        {
            _cssService = cssService;
            // Subscribe to changes in the service
            _cssService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CssService.CssContent))
                {
                    OnPropertyChanged(nameof(CssContent));
                }
            };
        }
    }
}