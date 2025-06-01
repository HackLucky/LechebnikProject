using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System.Data;
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
            string query = "SELECT * FROM Prescriptions";
            Prescriptions = DatabaseHelper.ExecuteQuery(query);
        }
    }
}