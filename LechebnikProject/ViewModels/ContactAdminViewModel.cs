using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ContactAdminViewModel : BaseViewModel
    {
        private string _messageText;
        public ObservableCollection<Message> Messages { get; set; }
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged(); }
        }

        public ICommand SendCommand { get; }
        public ICommand GoBackCommand { get; }

        public ContactAdminViewModel()
        {
            LoadMessages();
            SendCommand = new RelayCommand(Send);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadMessages()
        {
            string query = "SELECT m.*, u.Login AS SenderLogin FROM Messages m JOIN Users u ON m.SenderId = u.UserId WHERE (m.SenderId = @UserId AND m.ReceiverId = 1) OR (m.SenderId = 1 AND m.ReceiverId = @UserId)";
            var parameters = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            Messages = new ObservableCollection<Message>();
            foreach (DataRow row in dataTable.Rows)
            {
                Messages.Add(new Message
                {
                    SenderId = Convert.ToInt32(row["SenderId"]),
                    MessageText = row["MessageText"].ToString(),
                    SendDate = Convert.ToDateTime(row["SendDate"]),
                    SenderLogin = row["SenderLogin"].ToString()
                });
            }
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
            LoadMessages();
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