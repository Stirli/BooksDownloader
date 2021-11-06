using AngleSharp;
using AngleSharp.Dom;
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
            var config = Configuration.Default.WithDefaultLoader().WithJs();
            var context = BrowsingContext.New(config);
            document = await context.OpenAsync(Path.ToString()).WaitUntilAvailable();
            var divSelector = ".book-id ~ div:nth-of-type(2)";
            var bookTitleSelector = ".book-heading";
            var div = document.QuerySelector(divSelector);
            var title = div.QuerySelector(bookTitleSelector);
            var last = div.QuerySelector(".reader-pagination");
            Show = $"{title.TextContent} {last}";
        }

        private string consoleCommand;

        public string ConsoleCommand { get => consoleCommand; set => SetProperty(ref consoleCommand, value); }

        private string consoleLog;

        public string ConsoleLog { get => consoleLog; set => SetProperty(ref consoleLog, value); }

        private RelayCommand sendConsoleCommand;
        private AngleSharp.Dom.IDocument document;

        public ICommand SendConsoleCommand
        {
            get
            {
                if (sendConsoleCommand == null)
                {
                    sendConsoleCommand = new RelayCommand(SendConsole);
                }

                return sendConsoleCommand;
            }
        }

        private void SendConsole()
        {
            object result = null;
            try
            {
                result= document.ExecuteScript(ConsoleCommand);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            ConsoleLog += "\n" + result;
        }
    }
}