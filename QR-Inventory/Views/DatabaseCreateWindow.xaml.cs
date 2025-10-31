using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace QR_Inventory.Views
{
    public partial class DatabaseCreateWindow : Window
    {
        private readonly string configPath;

        public DatabaseCreateWindow()
        {
            InitializeComponent();

            // 📁 Получаем корень проекта (на 3 уровня выше bin/Debug/net8.0-windows)
            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!
                                               .Parent!.Parent!.Parent!.FullName;

            // 📂 Папка Database в корне проекта
            string defaultFolder = Path.Combine(projectRoot, "Database");
            Directory.CreateDirectory(defaultFolder);

            // 📄 Путь к конфигу (в корне проекта)
            configPath = Path.Combine(projectRoot, "config.json");

            // 🔹 Предлагаем путь по умолчанию
            PathBox.Text = defaultFolder;
            FileNameBox.Text = "QRInventory.db";
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Выберите путь для базы данных",
                Filter = "SQLite Database (*.db)|*.db",
                FileName = "QRInventory.db"
            };

            if (dialog.ShowDialog() == true)
            {
                PathBox.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
                FileNameBox.Text = System.IO.Path.GetFileName(dialog.FileName);
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string folder = PathBox.Text.Trim();
                string file = FileNameBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(file))
                {
                    MessageBox.Show("Укажите корректный путь и имя файла!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string fullPath = Path.Combine(folder, file);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // 🔹 создаём строку подключения
                string connectionString = $"Data Source={fullPath};";

                // 🔹 создаём БД и таблицы
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
CREATE TABLE IF NOT EXISTS USERS (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    USERNAME TEXT UNIQUE NOT NULL,
    PASSWORD_HASH TEXT NOT NULL,
    FULLNAME TEXT,
    ROLE TEXT NOT NULL,
    LAST_LOGIN TEXT
);

CREATE TABLE IF NOT EXISTS DEPARTMENTS (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    NAME TEXT NOT NULL,
    LOCATION TEXT
);

CREATE TABLE IF NOT EXISTS WORKPLACES (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    DEPARTMENT_ID INTEGER,
    RESPONSIBLE_ID INTEGER,
    NAME TEXT,
    DESCRIPTION TEXT,
    LAST_CHECK TEXT,
    FOREIGN KEY (DEPARTMENT_ID) REFERENCES DEPARTMENTS(ID),
    FOREIGN KEY (RESPONSIBLE_ID) REFERENCES USERS(ID)
);

CREATE TABLE IF NOT EXISTS INVENTORY (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    WORKPLACE_ID INTEGER,
    CATEGORY TEXT,
    NAME TEXT,
    MODEL TEXT,
    SERIAL_NUMBER TEXT,
    INVENTORY_NUMBER TEXT,
    PURCHASE_DATE TEXT,
    WARRANTY_UNTIL TEXT,
    STATUS TEXT,
    NOTES TEXT,
    FOREIGN KEY (WORKPLACE_ID) REFERENCES WORKPLACES(ID)
);

CREATE TABLE IF NOT EXISTS MAINTENANCE (
    ID INTEGER PRIMARY KEY AUTOINCREMENT,
    INVENTORY_ID INTEGER,
    DATE_PERFORMED TEXT,
    TYPE TEXT,
    DESCRIPTION TEXT,
    PERFORMED_BY TEXT,
    NEXT_CHECK TEXT,
    FOREIGN KEY (INVENTORY_ID) REFERENCES INVENTORY(ID)
);
";
                    using (var cmd = new SqliteCommand(sql, connection))
                        cmd.ExecuteNonQuery();

                    connection.Close();
                }

                // 🔹 Сохраняем конфигурацию
                SaveDatabaseConfig(fullPath);

                MessageBox.Show("База данных успешно создана и добавлена в конфигурацию!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании базы данных:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveDatabaseConfig(string dbPath)
        {
            try
            {
                var config = new
                {
                    DatabasePath = dbPath,
                    ConnectionType = dbPath.StartsWith(@"\\") ? "Network" : "Local",
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить конфигурацию:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
