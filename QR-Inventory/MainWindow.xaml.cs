using System;
using System.Reflection;
using System.Windows;

namespace QR_Inventory
{
    public partial class MainWindow : Window
    {
        public string AppVersion { get; }

        public MainWindow()
        {
            InitializeComponent();

            // 🔹 Получаем реальную версию из сборки
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            AppVersion = $"ver. {version}";

            DataContext = this;
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

        private void SetDarkTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ApplyTheme("DarkTheme"); // теперь мгновенно
        }

        private void SetLightTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ApplyTheme("LightTheme"); // теперь мгновенно
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

    }
}
