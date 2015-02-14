using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace QiQiaoBan.Helpers
{
    public class NavigationService : INavigationService
    {
        private Frame _mainFrame;

        public event NavigatingCancelEventHandler Navigating;

        public void Navigate(Type sourcePageType)
        {
            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(sourcePageType);
            }
        }

        public void Navigate(Type sourcePageType, object param)
        {
            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(sourcePageType, param);
            }
        }

        public void GoBack()
        {
            if (EnsureMainFrame() && _mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

        private bool EnsureMainFrame()
        {
            if (_mainFrame != null)
            {
                return true;
            }

            _mainFrame = Window.Current.Content as Frame;

            if (_mainFrame != null)
            {
                _mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };
                return true;
            }

            return false;
        }
    }
}
