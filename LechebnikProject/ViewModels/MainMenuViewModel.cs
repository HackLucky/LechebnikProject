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

        private bool CanGoToAdminPanel(object parameter)
        {
            return CurrentUser?.Role == "Admin";
        }

        public MainMenuViewModel()
        {
            try
            {
                GoToMedicineListCommand = new RelayCommand(o => WindowManager.ShowWindow<MedicineListWindow>());
                GoToCartCommand = new RelayCommand(o => WindowManager.ShowWindow<CartWindow>());
                GoToAddMedicineCommand = new RelayCommand(o => WindowManager.ShowWindow<AddMedicineWindow>());
                GoToReportsCommand = new RelayCommand(o => WindowManager.ShowWindow<ReportsWindow>());
                GoToPrescriptionsCommand = new RelayCommand(o => WindowManager.ShowWindow<PrescriptionsWindow>());
                GoToProfileCommand = new RelayCommand(o => WindowManager.ShowWindow<ProfileWindow>());
                GoToContactAdminCommand = new RelayCommand(o => WindowManager.ShowWindow<ContactAdminWindow>());
                GoToAdminPanelCommand = new RelayCommand(o => WindowManager.ShowWindow<AdminPanelWindow>(), CanGoToAdminPanel);
                ExitCommand = new RelayCommand(o => WindowManager.CloseAllWindows());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}