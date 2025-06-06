﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using LechebnikProject.Helpers;
using LechebnikProject.Models;

namespace LechebnikProject.ViewModels
{
    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public User Authenticate()
        {
            if (!ValidationHelper.IsNotEmpty(Login) || !ValidationHelper.IsNotEmpty(Password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            string query = "SELECT * FROM Users WHERE Login = @Login";
            SqlParameter[] parameters = { new SqlParameter("@Login", Login) };

            try
            {
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Некорректный логин или пароль.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                DataRow row = dataTable.Rows[0];
                string hashedPassword = row["PasswordHash"].ToString();
                string status = row["Status"].ToString();

                if (status == "Blocked")
                {
                    MessageBox.Show($"Ваш аккаунт заблокирован. Обратитесь к администратору: andrej_sok@mail.ru", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                if (!PasswordHasher.VerifyPassword(Password, hashedPassword))
                {
                    MessageBox.Show("Неверный логин или пароль.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }

                User user = new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    LastName = row["LastName"].ToString(),
                    FirstName = row["FirstName"].ToString(),
                    MiddleName = row["MiddleName"].ToString(),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    Email = row["Email"].ToString(),
                    Position = row["Position"].ToString(),
                    PharmacyAddress = row["PharmacyAddress"].ToString(),
                    Login = row["Login"].ToString(),
                    PasswordHash = row["PasswordHash"].ToString(),
                    Role = row["Role"].ToString(),
                    Status = row["Status"].ToString()
                };

                MessageBox.Show("Аутентификация успешна.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                return user;
            }
            catch (Exception ex)
            {
                Logger.LogError("Ошибка при аутентификации.", ex);
                MessageBox.Show("Произошла ошибка при аутентификации.", "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}