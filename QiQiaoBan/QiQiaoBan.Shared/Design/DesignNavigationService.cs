using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QiQiaoBan.Design
{
    /// <summary>
    /// This class is intended to show something in Blend or in VS 
    /// It makes designing easier without starting app
    /// </summary>
    public class DesignNavigationService : INavigationService
    {
        public string CurrentPageKey
        {
            get
            {
                Debug.WriteLine("DesignNavigationService.CurrentPageKey");
                return String.Empty;
            }
        }

        public void GoBack()
        {
            Debug.WriteLine("DesignNavigationService.GoBack()");
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            Debug.WriteLine(String.Format("DesignNavigationService.NavigateTo(\"{0}\", {1})", pageKey, parameter.ToString()));
        }

        public void NavigateTo(string pageKey)
        {
            Debug.WriteLine(String.Format("DesignNavigationService.NavigateTo(\"{0}\")", pageKey));
        }
    }   
}
