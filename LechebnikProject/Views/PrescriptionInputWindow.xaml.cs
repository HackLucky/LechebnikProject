using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для PrescriptionInputWindow.xaml
    /// </summary>
    public partial class PrescriptionInputWindow : Window
    {
        public PrescriptionInputWindow()
        {
            InitializeComponent();
        }

        public PrescriptionInputWindow(Medicine medicine) : this()
        {
            DataContext = new PrescriptionInputViewModel(medicine);
        }
    }
}
