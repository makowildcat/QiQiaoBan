using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.ViewModel
{
    public class ViewModelLocator
    {
        public const string GAME_PAGEKEY = "GamePage";

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                DispatcherHelper.Initialize();

                SimpleIoc.Default.Register<IDataService>(() => new Design.DesignDataService());

                SimpleIoc.Default.Register<INavigationService>(() => new Design.DesignNavigationService());

                SimpleIoc.Default.Register<IDialogService>(() => new Design.DesignDialogService());
            }

            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();

                SimpleIoc.Default.Register<INavigationService>(() =>
                    {
                        var navigationService = new NavigationService();
                        navigationService.Configure(GAME_PAGEKEY, typeof(GamePage));
                        return navigationService;
                    });

                SimpleIoc.Default.Register<IDialogService>(() =>
                {
                    return new DialogService();
                });
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }

        public GameViewModel Game
        {
            get
            {
                return SimpleIoc.Default.GetInstance<GameViewModel>();
            }
        }
    }
}
