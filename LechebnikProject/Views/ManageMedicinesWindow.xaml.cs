using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для ManageMedicinesWindow.xaml
    /// </summary>
    public partial class ManageMedicinesWindow : Window
    {
        public ManageMedicinesWindow()
        {
            InitializeComponent();
            DataContext = new ManageMedicinesViewModel();
        }
    }
}
