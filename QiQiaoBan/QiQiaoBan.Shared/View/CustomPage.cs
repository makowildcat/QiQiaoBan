using QiQiaoBan.Common;
using QiQiaoBan.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace QiQiaoBan.View
{
    public class CustomPage : Page
    {
        public IViewModel ViewModel { get; set; }

        private readonly NavigationHelper _navigationHelper;

        public CustomPage()
        {
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += NavigationHelperLoadState;
            _navigationHelper.SaveState += NavigationHelperSaveState;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
        }

        public void NavigationHelperLoadState(object sender, LoadStateEventArgs e)
        {
            ViewModel.LoadState(e);
        }

        public void NavigationHelperSaveState(object sender, SaveStateEventArgs e)
        {
            ViewModel.SaveState(e);
        }

    }
}
