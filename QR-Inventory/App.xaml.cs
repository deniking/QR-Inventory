using System;
using System.Linq;
using System.Windows;
using QR_Inventory.Properties;

namespace QR_Inventory
{
    public partial class App : Application
    {
        public App()
        {
            // Читаем последнюю тему из настроек
            string theme = Settings.Default.LastTheme;
            if (string.IsNullOrWhiteSpace(theme))
                theme = "DarkTheme";

            ApplyTheme(theme);
        }

        /// <summary>
        /// Подхватить тему (DarkTheme.xaml или LightTheme.xaml) и применить ко всему приложению
        /// </summary>
        public static void ApplyTheme(string themeName)
        {
            try
            {
                // Создаем словарь и указываем путь к теме
                var themeDict = new ResourceDictionary
                {
                    Source = new Uri(
                        $"pack://application:,,,/QR_Inventory;component/Themes/{themeName}.xaml",
                        UriKind.Absolute)
                };

                // Найдём предыдущий словарь темы (любой файл из папки Themes)
                var oldTheme = Current.Resources.MergedDictionaries
                    .FirstOrDefault(d =>
                        d.Source != null &&
                        d.Source.OriginalString.Contains("/Themes/", StringComparison.OrdinalIgnoreCase));

                if (oldTheme != null)
                {
                    Current.Resources.MergedDictionaries.Remove(oldTheme);
                }

                // Подключаем новый словарь
                Current.Resources.MergedDictionaries.Add(themeDict);

                // Сохраняем выбранную тему в Properties/Settings.settings
                Settings.Default.LastTheme = themeName;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка применения темы:\n{ex.Message}",
                    "Theme Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
