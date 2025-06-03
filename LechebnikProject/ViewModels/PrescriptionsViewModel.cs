using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class PrescriptionsViewModel : BaseViewModel
    {
        private DataTable _prescriptions;

        public DataTable Prescriptions
        {
            get => _prescriptions;
            set => SetProperty(ref _prescriptions, value);
        }

        public ICommand GoBackCommand { get; }

        public PrescriptionsViewModel()
        {
            LoadPrescriptions();
            GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadPrescriptions()
        {
            try
            {
                string query = "SELECT * FROM Prescriptions";
                Prescriptions = DatabaseHelper.ExecuteQuery(query);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}