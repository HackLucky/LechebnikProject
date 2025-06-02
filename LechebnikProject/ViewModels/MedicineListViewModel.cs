using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class MedicineListViewModel : BaseViewModel
    {
        private ObservableCollection<Medicine> _medicines;
        private string _searchText;

        public ObservableCollection<Medicine> Medicines
        {
            get => _medicines;
            set => SetProperty(ref _medicines, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadMedicines(_searchText);
            }
        }

        public ICommand ViewDetailsCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand AddToCartByPrescriptionCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GoToMainMenuCommand { get; }

        public MedicineListViewModel()
        {
            LoadMedicines();
            ViewDetailsCommand = new RelayCommand(ViewDetails);
            AddToCartCommand = new RelayCommand(AddToCart);
            AddToCartByPrescriptionCommand = new RelayCommand(AddToCartByPrescription);
            SearchCommand = new RelayCommand(Search);
            GoToMainMenuCommand = new RelayCommand(_ => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadMedicines(string searchText = "")
        {
            string query = @"
                SELECT m.*, man.Name AS ManufacturerName
                FROM Medicines m
                JOIN Manufacturers man ON m.ManufacturerId = man.ManufacturerId
                WHERE m.Name LIKE @SearchText OR m.SerialNumber LIKE @SearchText";

            var parameters = new[] { new SqlParameter("@SearchText", $"%{searchText}%") };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

            Medicines = new ObservableCollection<Medicine>();
            foreach (DataRow row in dataTable.Rows)
            {
                Medicines.Add(new Medicine
                {
                    MedicineId = Convert.ToInt32(row["MedicineId"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"]),
                    RequiresPrescription = Convert.ToBoolean(row["RequiresPrescription"]),
                    ManufacturerId = Convert.ToInt32(row["ManufacturerId"]),
                    ManufacturerName = row["ManufacturerName"].ToString()
                });
            }
        }

        private void ViewDetails(object parameter)
        {
            if (parameter is Medicine med)
            {
                WindowManager.ShowWindow<MedicineDetailsWindow>(w =>w.DataContext = new MedicineDetailsViewModel(med.MedicineId));
            }
        }

        private void AddToCart(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                if (medicine.RequiresPrescription)
                {
                    MessageBox.Show("Этот препарат требует рецепт. Используйте кнопку 'Добавить в корзину по рецепту'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var clientLoginWindow = new ClientLoginWindow();
                if (clientLoginWindow.ShowDialog() == true)
                {
                    // Клиент аутентифицирован, скидка уже в AppContext.CurrentClient
                    WindowManager.ShowWindow<QuantityInputWindow>(win => win.DataContext = new QuantityInputViewModel(medicine));
                }
                else
                {
                    // Пропуск аутентификации, скидка не применяется
                    AppContext.CurrentClient = null;
                    WindowManager.ShowWindow<QuantityInputWindow>(win => win.DataContext = new QuantityInputViewModel(medicine));
                }
            }
        }

        private void AddToCartByPrescription(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                WindowManager.ShowWindow<PrescriptionInputWindow>(win => win.DataContext = new PrescriptionInputViewModel(medicine));
            }
        }

        private void Search(object parameter) => LoadMedicines(SearchText);
    }
}