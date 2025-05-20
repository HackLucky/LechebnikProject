using Lechebnik.ViewModels;
using LechebnikProject.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        public ICommand ManageUsersCommand { get; }
        public ICommand ManageMedicinesCommand { get; }
        public ICommand ManageReportsCommand { get; }
        public ICommand GoBackCommand { get; }

        public AdminPanelViewModel()
        {
            ManageUsersCommand = new RelayCommand(ManageUsers);
            ManageMedicinesCommand = new RelayCommand(ManageMedicines);
            ManageReportsCommand = new RelayCommand(ManageReports);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void ManageUsers(object parameter)
        {
            var manageUsersWindow = new ManageUsersWindow();
            manageUsersWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is AdminPanelWindow))?.Close();
        }

        private void ManageMedicines(object parameter)
        {
            var manageMedicinesWindow = new ManageMedicinesWindow();
            manageMedicinesWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is AdminPanelWindow))?.Close();
        }

        private void ManageReports(object parameter)
        {
            var manageReportsWindow = new ManageReportsWindow();
            manageReportsWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is AdminPanelWindow))?.Close();
        }

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is AdminPanelWindow))?.Close();
            Application.Current.MainWindow = mainMenuWindow;
        }
    }
}