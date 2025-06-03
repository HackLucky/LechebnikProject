using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        public ICommand ManageUsersCommand { get; }
        public ICommand ManageMedicinesCommand { get; }
        public ICommand ManageReportsCommand { get; }
        public ICommand GoBackCommand { get; }

        public AdminPanelViewModel()
        {
            try
            {
                ManageUsersCommand = new RelayCommand(o => WindowManager.ShowWindow<ManageUsersWindow>());
                ManageMedicinesCommand = new RelayCommand(o => WindowManager.ShowWindow<ManageMedicinesWindow>());
                ManageReportsCommand = new RelayCommand(o => WindowManager.ShowWindow<ManageReportsWindow>());
                GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}