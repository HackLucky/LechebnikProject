using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using NLog;

namespace LechebnikProject.Helpers
{
    /// <summary>
    /// Класс для работы с базой данных, предоставляет методы для выполнения запросов.
    /// </summary>
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = AppConfigManager.GetConnectionString(); // Строка подключения из App.config

        /// <summary>
        /// Выполняет запрос с возвратом таблицы данных (SELECT).
        /// </summary>
        /// <param name="query">SQL-запрос</param>
        /// <param name="parameters">Параметры запроса для защиты от SQL-инъекций</param>
        /// <returns>Таблица с результатами запроса</returns>
        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open(); // Открытие соединения с базой данных
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                            command.Parameters.AddRange(parameters); // Добавление параметров для защиты от инъекций
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable); // Заполнение таблицы результатами запроса
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при выполнении запроса: {query}", ex); // Логирование ошибки
                throw; // Повторное выбрасывание исключения для обработки на верхнем уровне
            }
            return dataTable;
        }

        /// <summary>
        /// Выполняет запрос без возврата данных (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">SQL-запрос</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <returns>Количество затронутых строк</returns>
        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return command.ExecuteNonQuery(); // Выполнение запроса и возврат количества затронутых строк
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при выполнении команды: {query}", ex);
                throw;
            }
        }

        /// <summary>
        /// Выполняет запрос с возвратом одного значения (например, COUNT, MAX).
        /// </summary>
        /// <param name="query">SQL-запрос</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <returns>Единичное значение</returns>
        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return command.ExecuteScalar(); // Выполнение запроса и возврат первого значения
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при выполнении скалярного запроса: {query}", ex);
                throw;
            }
        }
    }
}