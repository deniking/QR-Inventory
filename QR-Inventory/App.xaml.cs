using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using QR_Inventory.Views; // для LoginWindow
using QR_Inventory.Properties;
using System.Collections.Generic;

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

                // 🔹 применяем сохранённую тему (по умолчанию тёмную)
                string theme = Settings.Default.LastTheme;
                if (string.IsNullOrWhiteSpace(theme))
                    theme = "DarkTheme";
                ApplyTheme(theme);

                // 🔹 показываем окно логина
                var loginWindow = new LoginWindow();
                bool? result = loginWindow.ShowDialog();

                // 🔹 если логин успешен — запускаем главное окно
                if (result == true)
                {
                    var mainWindow = new MainWindow();
                    this.MainWindow = mainWindow;

                    // ✅ возвращаем нормальный режим завершения — теперь закрытие MainWindow завершает программу
                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

                    mainWindow.Show();

                    // синхронизируем галочки меню при старте
                    UpdateThemeMenuChecks(theme);
                }
                else
                {
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

                // 🔄 обновляем интерфейс всех окон
                RefreshUI();

                // ✅ обновляем галочки в меню
                UpdateThemeMenuChecks(themeName);
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

        /// <summary>
        /// 🔄 Принудительно обновляет ресурсы всех открытых окон
        /// </summary>
        public static void RefreshUI()
        {
            foreach (Window window in Current.Windows)
            {
                window.Resources.MergedDictionaries.Clear();

                foreach (var dict in Current.Resources.MergedDictionaries)
                {
                    window.Resources.MergedDictionaries.Add(dict);
                }

                window.InvalidateVisual();
            }
        }

        /// <summary>
        /// ✅ Обновляет состояние галочек (IsChecked) у пунктов меню "Светлая/Тёмная тема"
        /// </summary>
        private static void UpdateThemeMenuChecks(string themeName)
        {
            foreach (Window window in Current.Windows)
            {
                foreach (var menuItem in window.FindVisualChildren<System.Windows.Controls.MenuItem>())
                {
                    if (menuItem.Header?.ToString() == "Тёмная тема")
                        menuItem.IsChecked = themeName.Equals("DarkTheme", StringComparison.OrdinalIgnoreCase);
                    else if (menuItem.Header?.ToString() == "Светлая тема")
                        menuItem.IsChecked = themeName.Equals("LightTheme", StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    /// <summary>
    /// Вспомогательный метод для обхода визуального дерева (поиск MenuItem'ов)
    /// </summary>
    public static class VisualTreeHelperExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T typedChild)
                    yield return typedChild;

                foreach (var subChild in FindVisualChildren<T>(child))
                    yield return subChild;
            }
        }
    }
}
