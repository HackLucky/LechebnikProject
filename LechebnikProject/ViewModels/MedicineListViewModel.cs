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
        public ICommand GoToCartCommand { get; }
        public ICommand GoBackCommand { get; }

        public MedicineListViewModel()
        {
            LoadMedicines();
            ViewDetailsCommand = new RelayCommand(ViewDetails);
            AddToCartCommand = new RelayCommand(AddToCart);
            AddToCartByPrescriptionCommand = new RelayCommand(AddToCartByPrescription);
            SearchCommand = new RelayCommand(Search);
            GoToCartCommand = new RelayCommand(GoToCart);
            GoBackCommand = new RelayCommand(GoBack);
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
            if (parameter is Medicine medicine)
            {
                var medicineDetailsWindow = new MedicineDetailsWindow(medicine.MedicineId);
                medicineDetailsWindow.Show();
                (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MedicineListWindow))?.Close();
                Application.Current.MainWindow = medicineDetailsWindow;
            }
        }

        private void AddToCart(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                var clientAuthWindow = new ClientAuthWindow();
                Client authenticatedClient = null;
                if (clientAuthWindow.ShowDialog() == true)
                {
                    authenticatedClient = clientAuthWindow.AuthenticatedClient;
                }

                var quantityInputWindow = new QuantityInputWindow(medicine);
                if (quantityInputWindow.ShowDialog() == true)
                {
                    int quantity = (quantityInputWindow.DataContext as QuantityInputViewModel).Quantity;
                    string query = "INSERT INTO CartItems (UserId, MedicineId, Quantity, IsByPrescription) VALUES (@UserId, @MedicineId, @Quantity, @IsByPrescription)";
                    var parameters = new[]
                    {
                        new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                        new SqlParameter("@MedicineId", medicine.MedicineId),
                        new SqlParameter("@Quantity", quantity),
                        new SqlParameter("@IsByPrescription", false)
                    };
                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                    if (authenticatedClient != null)
                    {
                        // Сохраняем информацию о клиенте для применения скидки
                        AppContext.CurrentClient = authenticatedClient;
                    }
                    MessageBox.Show("Препарат добавлен в корзину!");
                    (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is QuantityInputWindow))?.Close();
                }
            }
        }

        private void AddToCartByPrescription(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                var prescriptionInputWindow = new PrescriptionInputWindow(medicine);
                prescriptionInputWindow.Show();
                (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MedicineListWindow))?.Close();
                Application.Current.MainWindow = prescriptionInputWindow;
            }
        }

        private void Search(object parameter) => LoadMedicines(SearchText);

        private void GoToCart(object parameter)
        {
            var cartWindow = new CartWindow();
            cartWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MedicineListWindow))?.Close();
            Application.Current.MainWindow = cartWindow;
        }

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MedicineListWindow))?.Close();
            Application.Current.MainWindow = mainMenuWindow;
        }
    }
}