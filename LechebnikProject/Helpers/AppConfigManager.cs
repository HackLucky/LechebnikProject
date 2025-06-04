using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Xml;

public static class AppConfigManager
{
    private static readonly string UserConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LechebnikProjectConnection", "user.config");

    public static void UpdateConnectionString(
        string server,
        string database,
        bool useWindowsAuth,
        string username = null,
        string password = null)
    {
        try
        {
            string connectionString = useWindowsAuth
                ? $"Server={server};Database={database};Trusted_Connection=True;"
                : $"Server={server};Database={database};User Id={username};Password={password};";

            var doc = new XmlDocument();
            XmlElement root = doc.CreateElement("configuration");
            doc.AppendChild(root);

            XmlElement connStrings = doc.CreateElement("connectionStrings");
            root.AppendChild(connStrings);

            XmlElement addElem = doc.CreateElement("add");
            addElem.SetAttribute("name", "DefaultConnection");
            addElem.SetAttribute("connectionString", connectionString);
            addElem.SetAttribute("providerName", "System.Data.SqlClient");
            connStrings.AppendChild(addElem);

            string dir = Path.GetDirectoryName(UserConfigPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            doc.Save(UserConfigPath);

            UpdateConfigurationManager(connectionString);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private static void UpdateConfigurationManager(string connectionString)
    {
        try
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString = connectionString;
            config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ProviderName = "System.Data.SqlClient";
            config.Save(ConfigurationSaveMode.Modified, true);
            ConfigurationManager.RefreshSection("connectionStrings");
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    public static string GetConnectionString()
    {
        if (File.Exists(UserConfigPath))
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(UserConfigPath);
                XmlNode node = doc.SelectSingleNode("/configuration/connectionStrings/add[@name='DefaultConnection']");
                if (node != null && node.Attributes?["connectionString"] != null)
                {
                    return node.Attributes["connectionString"].Value;
                }
            }
            catch (Exception ex){ MessageBox.Show($"Ошибка получения пользовательского файла: {ex.Message}", "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        return ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
    }

    public static void InitializeConnectionString()
    {
        try
        {
            string userConnection = GetConnectionString();
            if (!string.IsNullOrEmpty(userConnection))
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString = userConnection;
                ConfigurationManager.RefreshSection("connectionStrings");
            }
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}