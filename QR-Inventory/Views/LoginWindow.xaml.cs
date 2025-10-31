using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace QR_Inventory.Views
{
    public partial class LoginWindow : Window
    {
        private readonly string appFolder;
        private readonly string configPath;
        private readonly string logPath;
        private ConfigData config = new ConfigData();

        public LoginWindow()
        {
            InitializeComponent();

            appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QR_Inventory");

            configPath = Path.Combine(appFolder, "config.json");
            logPath = Path.Combine(appFolder, "logs.txt");

            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    config = JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();

                    if (config.RememberMe)
                    {
                        UsernameBox.Text = config.LastUser ?? "";
                        PasswordBox.Password = config.LastPassword ?? "";
                        RememberCheckBox.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки конфигурации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveConfig()
        {
            try
            {
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                if (RememberCheckBox.IsChecked == true)
                {
                    config.RememberMe = true;
                    config.LastUser = UsernameBox.Text;
                    config.LastPassword = PasswordBox.Password;
                }
                else
                {
                    config.RememberMe = false;
                    config.LastUser = "";
                    config.LastPassword = "";
                }

                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения конфигурации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WriteLog(string message)
        {
            try
            {
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                File.AppendAllText(logPath,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}");
            }
            catch { /* не мешаем работе */ }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            string dbPath = "C:/QRInventory/QRInventory.fdb";
            bool dbExists = File.Exists(dbPath);

            bool canLogin = false;

            if (!dbExists)
            {
                if (username == "admin" && password == "admin")
                    canLogin = true;
            }
            else
            {
                // позже будет проверка в БД
                if (username == "admin" && password == "admin")
                    canLogin = true;
            }

            if (canLogin)
            {
                SaveConfig();
                WriteLog($"Пользователь '{username}' вошёл в систему.");

                // диагностика
                // MessageBox.Show("DEBUG: Успех логина, ставлю DialogResult = true");

                this.DialogResult = true; // ключевая строка
                // окно само закроется после возврата из обработчика
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                WriteLog($"Неудачная попытка входа: {username}");
            }
        }
    }

    public class ConfigData
    {
        public bool RememberMe { get; set; }
        public string? LastUser { get; set; }
        public string? LastPassword { get; set; }

        // 🆕 язык храним коротко (RU, UA, EN)
        public string LastLanguage { get; set; } = "RU";
    }

}
