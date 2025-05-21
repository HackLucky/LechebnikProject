using System;
using System.Collections.Generic;
using System.Windows;

namespace LechebnikProject.Helpers
{
    public static class WindowManager
    {
        private readonly static Dictionary<Type, Window> _openWindows = new Dictionary<Type, Window>();

        public static void ShowWindow<T>(Action<T> configure = null) where T : Window, new()
        {
            if (_openWindows.ContainsKey(typeof(T)))
            {
                _openWindows[typeof(T)].Activate();
                return;
            }

            T window = new T();
            configure?.Invoke(window);
            window.Closed += (s, e) => _openWindows.Remove(typeof(T));
            _openWindows[typeof(T)] = window;
            window.Show();
            Application.Current.MainWindow = window;
        }

        public static void CloseWindow<T>() where T : Window
        {
            if (_openWindows.ContainsKey(typeof(T)))
            {
                _openWindows[typeof(T)].Close();
                _openWindows.Remove(typeof(T));
            }
        }

        public static void CloseAllWindows()
        {
            foreach (var window in _openWindows.Values)
            {
                window.Close();
            }
            _openWindows.Clear();
        }
    }
}