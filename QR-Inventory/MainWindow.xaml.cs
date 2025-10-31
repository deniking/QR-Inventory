using QR_Inventory.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace QR_Inventory
{
    public partial class MainWindow : Window
    {
        public string AppVersion { get; }

        private readonly string appFolder;
        private readonly string configPath;
        private QR_Inventory.Views.ConfigData config = new QR_Inventory.Views.ConfigData();

        public MainWindow()
        {
            InitializeComponent();

            // 🔹 Получаем версию сборки
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            AppVersion = $"ver. {version}";
            DataContext = this;

            // 🔹 Пути для конфигурации
            appFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QR_Inventory");

            configPath = Path.Combine(appFolder, "config.json");

            // 🔹 Загружаем настройки языка
            LoadConfig();

            // 🔹 При загрузке окна — выставляем галочки
            Loaded += (_, _) =>
            {
                // ====== Тема ======
                string theme = Properties.Settings.Default.LastTheme;
                bool isDark = theme.Equals("DarkTheme", StringComparison.OrdinalIgnoreCase);
                UpdateThemeChecks(isDark);

                // ====== Язык ======
                UpdateLanguageChecks(config.LastLanguage);
            };
        }

        // ==============================
        //     ОБЩИЕ КНОПКИ МЕНЮ
        // ==============================

        private void Settings_User_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Настройки пользователя (заглушка)");
        }

        

        private void Settings_Bot_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Настройки Telegram-бота (заглушка)");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"QR-Inventory\nВерсия {AppVersion}", "О программе");
        }

        // ==============================
        //     СМЕНА ЯЗЫКА
        // ==============================

        private void SetLanguage_UA_Click(object sender, RoutedEventArgs e)
        {
            config.LastLanguage = "UA";
            SaveConfig();
            UpdateLanguageChecks(config.LastLanguage);
            MessageBox.Show("Мова інтерфейсу: Українська (ще в розробці)", "Мова");
        }

        private void SetLanguage_EN_Click(object sender, RoutedEventArgs e)
        {
            config.LastLanguage = "EN";
            SaveConfig();
            UpdateLanguageChecks(config.LastLanguage);
            MessageBox.Show("Interface language: English (in progress)", "Language");
        }

        private void SetLanguage_RU_Click(object sender, RoutedEventArgs e)
        {
            config.LastLanguage = "RU";
            SaveConfig();
            UpdateLanguageChecks(config.LastLanguage);
            MessageBox.Show("Язык интерфейса: Русский (в разработке)", "Язык");
        }

        // ==============================
        //     ТЕМЫ ОФОРМЛЕНИЯ
        // ==============================

        private void SetDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ApplyTheme("DarkTheme");
            UpdateThemeChecks(isDark: true);
        }

        private void SetLightTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ApplyTheme("LightTheme");
            UpdateThemeChecks(isDark: false);
        }

        /// <summary>
        /// Обновляет галочки у пунктов "Тёмная / Светлая тема"
        /// </summary>
        private void UpdateThemeChecks(bool isDark)
        {
            MenuDarkTheme.IsChecked = isDark;
            MenuLightTheme.IsChecked = !isDark;
        }

        /// <summary>
        /// Обновляет галочки у пунктов "Языки"
        /// </summary>
        private void UpdateLanguageChecks(string lang)
        {
            LangUA.IsChecked = lang == "UA";
            LangEN.IsChecked = lang == "EN";
            LangRU.IsChecked = lang == "RU";
        }

        // ==============================
        //     Работа с config.json
        // ==============================

        private void LoadConfig()
        {
            try
            {
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    config = JsonSerializer.Deserialize<QR_Inventory.Views.ConfigData>(json)
                             ?? new QR_Inventory.Views.ConfigData();
                }
            }
            catch
            {
                config = new QR_Inventory.Views.ConfigData();
            }
        }

        private void SaveConfig()
        {
            try
            {
                if (!Directory.Exists(appFolder))
                    Directory.CreateDirectory(appFolder);

                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch
            {
                // не мешаем работе
            }
        }

        /// <summary>
        /// Универсальный обход меню (на случай вложенных Popup)
        /// </summary>
        private static IEnumerable<MenuItem> FindAllMenuItems(DependencyObject root)
        {
            if (root == null) yield break;

            if (root is MenuItem mi)
                yield return mi;

            foreach (object child in LogicalTreeHelper.GetChildren(root))
            {
                if (child is DependencyObject dep)
                {
                    foreach (var sub in FindAllMenuItems(dep))
                        yield return sub;
                }
            }
        }
        private void Settings_DB_Click(object sender, RoutedEventArgs e)
        {
            var dbWindow = new DatabaseCreateWindow
            {
                Owner = this
            };
            dbWindow.ShowDialog();
        }

    }
}
