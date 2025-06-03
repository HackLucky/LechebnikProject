using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ManageMedicinesWindow : Window
    {
        public ManageMedicinesWindow()
        {
            InitializeComponent();
            SearchTextBox.Focus();
            DataContext = new ManageMedicinesViewModel();
        }
    }
}
