using System.Windows;

namespace QR_Inventory
{
    public partial class MainWindow : Window
    {
        public string AppVersion { get; } = "1.0.0.0";

        public MainWindow()
        {
            InitializeComponent();
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
            App.ApplyTheme("DarkTheme");
        }

        private void SetLightTheme_Click(object sender, RoutedEventArgs e)
        {
            App.ApplyTheme("LightTheme");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("QR-Inventory\nВерсия 1.0.0.0", "О программе");
        }
    }
}
