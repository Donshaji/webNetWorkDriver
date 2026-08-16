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
        public partial class MainViewModel : ObservableObject
        {
            private readonly TcpFetcher _fetcher = new TcpFetcher();
            private readonly CssService _cssService;

        [ObservableProperty]
        private string host = "example.com";

        [ObservableProperty]
        private int port = 80;

        [ObservableProperty]
        private string path = "/";

        [ObservableProperty]
        private string response;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string cssContent;

        public MainViewModel(CssService cssService)
        {
            _cssService = cssService;
        }

        [RelayCommand]
            private async Task Fetch()
            {
                try
                {
                    IsBusy = true;
                    Response = "Connecting...";
                    Response =await _fetcher.FetchAsync(Host, Port, Path);
                    string htmlBody = _fetcher.GetHtmlBody(Response);
                    string css = _fetcher.ExtractCss(Response);
            }
                catch (Exception ex)
                {
                    Response = $"Error: {ex.Message}";
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
}