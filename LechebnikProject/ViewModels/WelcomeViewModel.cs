using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System.Threading.Tasks;
using System.Windows;

namespace LechebnikProject.ViewModels
{
    public class WelcomeViewModel
    {
        public WelcomeViewModel()
        {
            Task.Delay(2000).ContinueWith(t => Application.Current.Dispatcher.Invoke(() => WindowManager.ShowWindow<LoginWindow>()));
        }
    }
}