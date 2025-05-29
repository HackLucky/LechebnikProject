using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
            GoToMainMenuCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
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
            if (!(parameter is Medicine med)) return;

            // Закрываем только список, открываем детали
            WindowManager.ShowWindow<MedicineDetailsWindow>(w => w.DataContext = new MedicineDetailsViewModel(med.MedicineId));
        }

        private void AddToCart(object parameter)
        {
            if (!(parameter is Medicine med)) return;

            // 1) Открываем окно аутентификации и закрываем все остальные
            WindowManager.ShowWindow<ClientLoginWindow>(w => w.DataContext = new ClientLoginViewModel());

            // 2) Открываем окно ввода количества и снова закрываем всё остальное
            WindowManager.ShowWindow<QuantityInputWindow>(w => w.DataContext = new QuantityInputViewModel(med));

            // 3) Сохраняем в БД
            const string sql = @"
                INSERT INTO CartItems (UserId, MedicineId, Quantity, IsByPrescription)
                VALUES (@UserId, @MedicineId, @Quantity, @IsByPrescription)";

            var qtyVm = (Application.Current.Windows.OfType<QuantityInputWindow>().First().DataContext as QuantityInputViewModel);

            int quantity = qtyVm?.Quantity ?? 1;

            var parameters = new[]
            {
                new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                new SqlParameter("@MedicineId", med.MedicineId),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@IsByPrescription", false)
            };
            DatabaseHelper.ExecuteNonQuery(sql, parameters);

            MessageBox.Show("Препарат добавлен в корзину!");

            // 4) Переходим в корзину, закрывая все прочие окна:
            WindowManager.ShowWindow<CartWindow>();
        }

        private void AddToCartByPrescription(object parameter)
        {
            if (!(parameter is Medicine med)) return;
            WindowManager.ShowWindow<PrescriptionInputWindow>(w => w.DataContext = new PrescriptionInputViewModel(med));
        }

        private void Search(object parameter) => LoadMedicines(SearchText);
    }
}