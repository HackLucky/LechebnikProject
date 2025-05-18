using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ContactAdminViewModel : BaseViewModel
    {
        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged(); }
        }

        public ICommand SendCommand { get; }
        public ICommand GoBackCommand { get; }

        public ContactAdminViewModel()
        {
            SendCommand = new RelayCommand(Send);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void Send(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MessageText))
            {
                MessageBox.Show("Введите сообщение.");
                return;
            }

            string query = "INSERT INTO Messages (SenderId, ReceiverId, MessageText, SendDate) VALUES (@SenderId, 1, @MessageText, @SendDate)";
            SqlParameter[] parameters = {
                new SqlParameter("@SenderId", AppContext.CurrentUser.UserId),
                new SqlParameter("@MessageText", MessageText),
                new SqlParameter("@SendDate", DateTime.Now)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Сообщение отправлено!");
                MessageText = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при отправке сообщения. {ex}");
            }
        }

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is ContactAdminWindow))?.Close();
            Application.Current.MainWindow = mainMenuWindow;
        }
    }
}