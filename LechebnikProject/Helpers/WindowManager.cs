using System;
using System.Linq;
using System.Windows;

namespace LechebnikProject.Helpers
{
    public static class WindowManager
    {
        public static void ShowWindow<T>(Action<T> configure = null) where T : Window, new()
        {
            var newWin = new T();
            configure?.Invoke(newWin);
            newWin.Show();

            Application.Current.MainWindow = newWin;

            foreach (Window w in Application.Current.Windows.OfType<Window>().ToList())
            {
                if (w != newWin) w.Close();
            }
        }

        public static void ShowWindowClosingCurrent<T>(Action<T> configure = null) where T : Window, new()
        {
            var previous = Application.Current.MainWindow;

            var newWin = new T();
            configure?.Invoke(newWin);
            newWin.Show();
            Application.Current.MainWindow = newWin;

            if (previous != null && previous != newWin) previous.Close();
        }

        public static void ShowWindowWithoutClosing<T>(Action<T> configure = null)
            where T : Window, new()
        {
            var existing = Application.Current.Windows.OfType<T>().FirstOrDefault();
            if (existing != null)
            {
                existing.Activate();
                return;
            }
            var win = new T();
            configure?.Invoke(win);
            win.Show();
        }

        public static void CloseWindow<T>() where T : Window
        {
            var existing = Application.Current.Windows.OfType<T>().FirstOrDefault();
            existing?.Close();
        }

        public static void CloseAllWindows()
        {
            foreach (Window w in Application.Current.Windows.OfType<Window>().ToList()) w.Close();
            Application.Current.Shutdown();
        }
    }
}
