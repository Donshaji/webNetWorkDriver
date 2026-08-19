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
            private readonly MetaDataService _metaDataService;

        [ObservableProperty]
        private string host = "example.com";

        [ObservableProperty]
        private int port = 80;

        [ObservableProperty]
        private string path = "/";

        [ObservableProperty]
        private string response = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string cssContent = string.Empty;

        public MainViewModel(CssService cssService, MetaDataService metaDataService)
        {
            _cssService = cssService;
            _metaDataService = metaDataService;
        }

        [RelayCommand]
            private async Task Fetch()
            {
                try
                {
                    IsBusy = true;
                    Response = "Connecting...";
                    Response = await _fetcher.FetchAsync(Host, Port, Path);
                    string htmlBody = _fetcher.GetHtmlBody(Response);
                    string css = _fetcher.ExtractCss(htmlBody);
                    _cssService.CssContent = css;
                    string metaData = _fetcher.ExtractAllMetadata(htmlBody);
                    _metaDataService.MetaDataContent = metaData;
            }
                catch (OperationCanceledException)
                {
                    Response = $"Error: Connection to {Host}:{Port} timed out.";
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