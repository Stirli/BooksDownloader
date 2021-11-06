using Microsoft.Toolkit.Mvvm.Input;
using System;

namespace BooksDownloader.ViewModels.MainWindow
{
    public interface IMainViewModel
    {
        string Title { get; }
        Uri Path { get; set; }
        string Show { get; set; }
        AsyncRelayCommand Download { get; set; }
    }
}