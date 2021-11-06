using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.RenderTree;
using AngleSharp.Dom;
using AngleSharp.Io;
using AngleSharp.Js;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BooksDownloader.ViewModels.MainWindow
{
    public class MockMainViewModel : ObservableObject, IMainViewModel
    {
        private const string PATH = "https://litnet.com/ru/reader/kogda-doroga-sama-vybrala-tebya-b175796";

        private Uri _path;
        private string _show;

        public string Title => "Books Downloader";
        public string Show { get => _show; set => SetProperty(ref _show, value); }

        public Uri Path { get => _path; set => SetProperty(ref _path, value); }

        public AsyncRelayCommand Download { get; set; }

        public MockMainViewModel()
        {
            Path = new Uri(PATH);
            Download = new AsyncRelayCommand(DownloadExecute);
        }

        private async Task DownloadExecute()
        {
            LoaderOptions lo = new LoaderOptions();
            
            lo.IsResourceLoadingEnabled = true;
            lo.Filter = request => request.Address.Host.Contains("litnet.com");
            var config = Configuration.Default
                                      .WithJs()
                                      .WithCss()
                                      .WithRenderDevice(new DefaultRenderDevice
                                      {
                                          DeviceHeight = 768,
                                          DeviceWidth = 1024,
                                      })
                                      .WithEventLoop()
                                      .WithDefaultCookies()
                                      .WithDefaultLoader(lo);
            var context = BrowsingContext.New(config);
            document = await context.OpenAsync(Path.ToString()).WaitUntilAvailable();

            var divSelector = ".book-id ~ div:nth-of-type(2)";
            var bookTitleSelector = ".book-heading";
            var div = document.QuerySelector(divSelector);
            var title = div.QuerySelector(bookTitleSelector);

            await UpdateTextAsync(div);
        }

        private async Task UpdateTextAsync(IElement div)
        {
            var tree = document.DefaultView.Render();
            var node = tree.Find(div);
            await node.DownloadResources();
            var last = div.QuerySelector(".reader-text");
            Show = $"{last?.TextContent}";
        }

        private string consoleCommand;

        public string ConsoleCommand { get => consoleCommand; set => SetProperty(ref consoleCommand, value); }

        private string consoleLog;

        public string ConsoleLog { get => consoleLog; set => SetProperty(ref consoleLog, value); }

        private AsyncRelayCommand sendConsoleCommand;
        private AngleSharp.Dom.IDocument document;

        public ICommand SendConsoleCommand
        {
            get
            {
                if (sendConsoleCommand == null)
                {
                    sendConsoleCommand = new AsyncRelayCommand(SendConsoleAsync);
                }

                return sendConsoleCommand;
            }
        }

        private async Task SendConsoleAsync()
        {
            object result = null;
            try
            {
                result = document.ExecuteScript(ConsoleCommand);
                await UpdateTextAsync(document.Body);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            ConsoleLog += "\n" + result;
        }
    }
}