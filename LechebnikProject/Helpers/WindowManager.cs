// Helpers/WindowManager.cs
using System;
using System.Linq;
using System.Windows;

namespace LechebnikProject.Helpers
{
    public static class WindowManager
    {
        /// <summary>
        /// Открыть новое окно и закрыть все остальные.
        /// </summary>
        public static void ShowWindow<T>(Action<T> configure = null)
            where T : Window, new()
        {
            // 1) Создаём и показываем ЦЕЛЕВОЕ окно
            var newWin = new T();
            configure?.Invoke(newWin);
            newWin.Show();

            // Делаем его главным
            Application.Current.MainWindow = newWin;

            // 2) Только после этого закрываем все окна, кроме него
            foreach (Window w in Application.Current.Windows
                                          .OfType<Window>()
                                          .ToList())
            {
                if (w != newWin)
                    w.Close();
            }
        }

        /// <summary>
        /// Открыть новое окно, закрыв текущее MainWindow (без рисков ShutdownMode).
        /// </summary>
        public static void ShowWindowClosingCurrent<T>(Action<T> configure = null)
            where T : Window, new()
        {
            // 1) Помним предыдущее окно до создания нового
            var previous = Application.Current.MainWindow;

            // 2) Создаём и показываем новое
            var newWin = new T();
            configure?.Invoke(newWin);
            newWin.Show();
            Application.Current.MainWindow = newWin;

            // 3) Закрываем старое
            if (previous != null && previous != newWin)
                previous.Close();
        }

        /// <summary>
        /// Открыть новое окно, не закрывая других.
        /// </summary>
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

        /// <summary>
        /// Закрыть окно указанного типа (если открыто).
        /// </summary>
        public static void CloseWindow<T>() where T : Window
        {
            var existing = Application.Current.Windows.OfType<T>().FirstOrDefault();
            existing?.Close();
        }

        /// <summary>
        /// Закрыть все окна и завершить приложение.
        /// </summary>
        public static void CloseAllWindows()
        {
            foreach (Window w in Application.Current.Windows.OfType<Window>().ToList())
                w.Close();
            Application.Current.Shutdown();
        }
    }
}
