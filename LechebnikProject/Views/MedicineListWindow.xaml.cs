using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для MedicineListWindow.xaml
    /// </summary>
    public partial class MedicineListWindow : Window
    {
        public MedicineListWindow()
        {
            InitializeComponent();
            DataContext = new MedicineListViewModel();
        }
    }
}
