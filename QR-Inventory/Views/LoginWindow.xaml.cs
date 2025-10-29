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
        private ConfigData config;

        public LoginWindow()
        {
            InitializeComponent();

            // 📂 Папка AppData\Local\QR_Inventory
            appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QR_Inventory");
            configPath = Path.Combine(appFolder, "config.json");
            logPath = Path.Combine(appFolder, "logs.txt");

            LoadConfig();
        }

        // ========================= Загрузка конфигурации =========================
        private void LoadConfig()
        {
            try
            {
                // Создаём папку, если её нет
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
                else
                {
                    config = new ConfigData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки конфигурации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                config = new ConfigData();
            }
        }

        // ========================= Сохранение конфигурации =========================
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

        // ========================= Запись логов =========================
        private void WriteLog(string message)
        {
            try
            {
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
                File.AppendAllText(logPath, logLine + Environment.NewLine);
            }
            catch
            {
                // Игнорируем ошибки логирования, чтобы не мешать работе приложения
            }
        }

        // ========================= Авторизация =========================
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // путь к БД (пока тестовый)
            string dbPath = "C:/QRInventory/QRInventory.fdb";
            bool dbExists = File.Exists(dbPath);

            bool canLogin = false;

            // если базы нет — разрешаем только admin/admin
            if (!dbExists)
            {
                if (username == "admin" && password == "admin")
                    canLogin = true;
            }
            else
            {
                // здесь позже будет реальная проверка в таблице users
                if (username == "admin" && password == "admin")
                    canLogin = true;
            }

            if (canLogin)
            {
                SaveConfig();

                // создаём лог при первом входе
                WriteLog($"Пользователь '{username}' вошёл в систему.");

                var main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                WriteLog($"Неудачная попытка входа: {username}");
            }
        }
    }

    // ========================= Модель конфигурации =========================
    public class ConfigData
    {
        public bool RememberMe { get; set; }
        public string? LastUser { get; set; }
        public string? LastPassword { get; set; }
    }
}
