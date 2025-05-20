using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Linq;
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
            var medicineListWindow = new MedicineListWindow();
            medicineListWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = medicineListWindow;
        }

        private void GoToCart(object parameter)
        {
            var cartWindow = new CartWindow();
            cartWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = cartWindow;
        }

        private void GoToAddMedicine(object parameter)
        {
            var addMedicineWindow = new AddMedicineWindow();
            addMedicineWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = addMedicineWindow;
        }

        private void GoToReports(object parameter)
        {
            var reportsWindow = new ReportsWindow();
            reportsWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = reportsWindow;
        }

        private void GoToPrescriptions(object parameter)
        {
            var prescriptionsWindow = new PrescriptionsWindow();
            prescriptionsWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = prescriptionsWindow;
        }

        private void GoToProfile(object parameter)
        {
            var profileWindow = new ProfileWindow();
            profileWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = profileWindow;
        }

        private void GoToContactAdmin(object parameter)
        {
            var contactAdminWindow = new ContactAdminWindow();
            contactAdminWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = contactAdminWindow;
        }

        private void GoToAdminPanel(object parameter)
        {
            var adminPanelWindow = new AdminPanelWindow();
            adminPanelWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainMenuWindow))?.Close();
            Application.Current.MainWindow = adminPanelWindow;
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