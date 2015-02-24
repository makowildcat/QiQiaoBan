using QiQiaoBan.Common;
using QiQiaoBan.View;
using QiQiaoBan.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace QiQiaoBan
{
    public sealed partial class MainPage : CustomPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = this.DataContext as IViewModel;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
    }
}
