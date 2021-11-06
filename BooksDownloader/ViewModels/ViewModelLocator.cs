using BooksDownloader.ViewModels.MainWindow;
using System.ComponentModel;
using System.Windows;

namespace BooksDownloader.ViewModels
{
    public class ViewModelLocator
    {
        private DependencyObject dummy = new DependencyObject();

        public IMainViewModel MainViewModel
        {
            get
            {
                if (IsInDesignMode())
                {
                    return new MockMainViewModel();
                }

                return new MockMainViewModel();
            }
        }

        private bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(dummy);
        }
    }
}
