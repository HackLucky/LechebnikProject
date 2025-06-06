﻿using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
            Messages = new ObservableCollection<Message>();
            LoadMessages();
            SendCommand = new RelayCommand(Send);
            GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadMessages()
        {
            string query = @"SELECT m.*, u.Login AS SenderLogin 
                FROM Messages m 
                JOIN Users u ON m.SenderId = u.UserId";
            var parameters = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            Messages.Clear();
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
                MessageBox.Show("Введите сообщение.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string query = @"INSERT INTO Messages (SenderId, ReceiverId, MessageText, SendDate) 
                VALUES (@SenderId, 1, @MessageText, @SendDate)";
            SqlParameter[] parameters = {
                new SqlParameter("@SenderId", AppContext.CurrentUser.UserId),
                new SqlParameter("@MessageText", MessageText),
                new SqlParameter("@SendDate", DateTime.Now)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                MessageText = string.Empty;
                LoadMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}