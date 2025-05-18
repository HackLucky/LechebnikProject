using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class MainMenuViewModel
    {
        public User CurrentUser => AppContext.CurrentUser;

        public ICommand GoToMedicineListCommand { get; }
        public ICommand GoToCartCommand { get; }
        public ICommand GoToAddMedicineCommand { get; }
        public ICommand GoToReportsCommand { get; }
        public ICommand GoToPrescriptionsCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand GoToContactAdminCommand { get; }
        public ICommand GoToAdminPanelCommand { get; }
        public ICommand ExitCommand { get; }

        public MainMenuViewModel()
        {
            GoToMedicineListCommand = new RelayCommand(GoToMedicineList);
            GoToCartCommand = new RelayCommand(GoToCart);
            GoToAddMedicineCommand = new RelayCommand(GoToAddMedicine);
            GoToReportsCommand = new RelayCommand(GoToReports);
            GoToPrescriptionsCommand = new RelayCommand(GoToPrescriptions);
            GoToProfileCommand = new RelayCommand(GoToProfile);
            GoToContactAdminCommand = new RelayCommand(GoToContactAdmin);
            GoToAdminPanelCommand = new RelayCommand(GoToAdminPanel, CanGoToAdminPanel);
            ExitCommand = new RelayCommand(Exit);
        }

        private void GoToMedicineList(object parameter)
        {
            var window = new MedicineListWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToCart(object parameter)
        {
            var window = new CartWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToAddMedicine(object parameter)
        {
            var window = new AddMedicineWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToReports(object parameter)
        {
            var window = new ReportsWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToPrescriptions(object parameter)
        {
            var window = new PrescriptionsWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToProfile(object parameter)
        {
            var window = new ProfileWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToContactAdmin(object parameter)
        {
            var window = new ContactAdminWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private void GoToAdminPanel(object parameter)
        {
            var window = new AdminPanelWindow();
            window.Show();
            (parameter as Window)?.Close();
        }

        private bool CanGoToAdminPanel(object parameter)
        {
            return CurrentUser?.Role == "Admin";
        }

        private void Exit(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}