using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace QR_Inventory
{
    public partial class MainWindow : Window
    {
        public string AppVersion { get; }

        public MainWindow()
        {
            InitializeComponent();

            // 🔹 Получаем версию сборки
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            AppVersion = $"ver. {version}";
            DataContext = this;

            // 🔹 Синхронизируем галочки при загрузке окна
            Loaded += (_, _) =>
            {
                string theme = Properties.Settings.Default.LastTheme;
                bool isDark = theme.Equals("DarkTheme", StringComparison.OrdinalIgnoreCase);
                UpdateThemeChecks(isDark);
            };
        }

        private void Settings_User_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Настройки пользователя (заглушка)");
        }

        private void Settings_DB_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Настройки базы данных (заглушка)");
        }

        private void Settings_Bot_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Настройки Telegram-бота (заглушка)");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"QR-Inventory\nВерсия {AppVersion}", "О программе");
        }

        private void SetLanguage_UA_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Мова інтерфейсу: Українська (ще в розробці)", "Мова");
        }

        private void SetLanguage_EN_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Interface language: English (in progress)", "Language");
        }

        private void SetLanguage_RU_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Язык интерфейса: Русский (в разработке)", "Язык");
        }

        // ================================
        //      Смена темы оформления
        // ================================

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
        /// Обновляет состояние галочек у пунктов "Светлая/Тёмная тема"
        /// </summary>
        private void UpdateThemeChecks(bool isDark)
        {
            foreach (var item in FindAllMenuItems(this))
            {
                if (item.Header?.ToString() == "Тёмная тема")
                    item.IsChecked = isDark;
                else if (item.Header?.ToString() == "Светлая тема")
                    item.IsChecked = !isDark;
            }
        }

        /// <summary>
        /// Универсальный обход меню (учитывает вложенные Popup)
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
    }
}
