﻿using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class EditMedicineWindow : Window
    {
        public EditMedicineWindow()
        {
            InitializeComponent();
            NameTextBox.Focus();
            DataContext = new EditMedicineViewModel();
        }
    }
}
