using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class PrescriptionInputWindow : Window
    {
        public PrescriptionInputWindow()
        {
            InitializeComponent();
            SeriesTextBox.Focus();
        }

        public PrescriptionInputWindow(Medicine medicine) : this()
        {
            DataContext = new PrescriptionInputViewModel(medicine);
        }
    }
}
