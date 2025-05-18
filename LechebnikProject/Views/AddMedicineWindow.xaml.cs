using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для AddMedicineWindow.xaml
    /// </summary>
    public partial class AddMedicineWindow : Window
    {
        public AddMedicineWindow()
        {
            InitializeComponent();
            DataContext = new AddMedicineViewModel();
        }
    }
}
