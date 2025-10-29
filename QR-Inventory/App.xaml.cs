using System;
using System.Linq;
using System.Windows;
using QR_Inventory.Views; // для LoginWindow
using QR_Inventory.Properties;

namespace QR_Inventory
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // 🚀 предотвращаем автоматическое завершение приложения после закрытия LoginWindow
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // 🔹 применяем тему
                string theme = Settings.Default.LastTheme;
                if (string.IsNullOrWhiteSpace(theme))
                    theme = "DarkTheme";
                ApplyTheme(theme);

                // 🔹 показываем логин модально
                var loginWindow = new LoginWindow();
                bool? result = loginWindow.ShowDialog();

                // 🔹 если логин успешен — запускаем главное окно
                if (result == true)
                {
                    var mainWindow = new MainWindow();
                    this.MainWindow = mainWindow; // важно: назначаем в Application

                    // ✅ возвращаем нормальный режим завершения — теперь закрытие MainWindow завершает программу
                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                    mainWindow.Show();
                }
                else
                {
                    // закрытие логина без входа или ошибка входа
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при запуске приложения:\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
            }
        }

        /// <summary>
        /// Применение темы оформления (DarkTheme.xaml / LightTheme.xaml и т.д.)
        /// </summary>
        public static void ApplyTheme(string themeName)
        {
            try
            {
                var themeDict = new ResourceDictionary
                {
                    Source = new Uri(
                        $"pack://application:,,,/QR_Inventory;component/Themes/{themeName}.xaml",
                        UriKind.Absolute)
                };

                // удаляем старую тему, если есть
                var oldTheme = Current.Resources.MergedDictionaries
                    .FirstOrDefault(d =>
                        d.Source != null &&
                        d.Source.OriginalString.Contains("/Themes/", StringComparison.OrdinalIgnoreCase));

                if (oldTheme != null)
                    Current.Resources.MergedDictionaries.Remove(oldTheme);

                // добавляем новую тему
                Current.Resources.MergedDictionaries.Add(themeDict);

                // сохраняем выбранную тему
                Settings.Default.LastTheme = themeName;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка применения темы:\n{ex.Message}",
                    "Theme Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
