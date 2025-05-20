using LechebnikProject.Models;
using LechebnikProject.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems => AppContext.CartItems;
        public ICommand RemoveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand GoBackCommand { get; }

        public CartViewModel()
        {
            RemoveCommand = new RelayCommand(Remove);
            ClearCommand = new RelayCommand(Clear);
            CheckoutCommand = new RelayCommand(Checkout);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void Remove(object parameter)
        {
            if (parameter is CartItem item)
                CartItems.Remove(item);
        }

        private void Clear(object parameter)
        {
            CartItems.Clear();
        }

        private void Checkout(object parameter)
        {
            MessageBox.Show("Оплата успешно выполнена!");
            CartItems.Clear();
            GoBack(parameter);
        }

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is CartWindow))?.Close();
            Application.Current.MainWindow = mainMenuWindow;
        }
    }
}