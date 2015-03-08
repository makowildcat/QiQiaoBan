using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using QiQiaoBan.Common;
using QiQiaoBan.Helpers;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace QiQiaoBan.ViewModel
{
    /// <summary>
    /// Game ViewModel handle game rules, timing, saving...
    /// </summary>
    public class GameViewModel : ViewModelBase, IViewModel
    {
        private const double DELTA_MARGIN = 20.0;
        private const int TIME_INTERVAL_SECOND = 1;

        private IDialogService _dialogService = null;
        private GalaSoft.MvvmLight.Views.INavigationService _navigationService = null;

        private DispatcherTimer dispatcherTime;
        private int _bestTime;
        private const string BestTimePropertyName = "BestTime";

        public const string NamePropertyName = "Name";
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(NamePropertyName, ref _name, value);
            }
        }

        public const string TimePropertyName = "Time";
        private int _time;
        public int Time
        {
            get
            {
                return _time;
            }
            set
            {
                Set(TimePropertyName, ref _time, value);
            }
        }
        
        public const string PiecesViewModelPropertyName = "PiecesViewModel";
        private PiecesViewModel _piecesViewModel;
        public PiecesViewModel PiecesViewModel
        {
            get
            {
                return _piecesViewModel;
            }
            set
            {
                Set(PiecesViewModelPropertyName, ref _piecesViewModel, value);
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the GameViewModel class
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public GameViewModel(GalaSoft.MvvmLight.Views.INavigationService navigationService, IDialogService dialogService)
        {
            Debug.WriteLine("GameViewModel.constructor");

            _dialogService = dialogService;
            _navigationService = navigationService;

            dispatcherTime = new DispatcherTimer();
            dispatcherTime.Interval = TimeSpan.FromSeconds(TIME_INTERVAL_SECOND);
            dispatcherTime.Tick += dispatcherTimeTick;

            MessengerInstance.Register<NotificationMessageAction>(this, finishGame);
        }

        /// <summary>
        /// LoadState invoked when binded Page is about to displayed
        /// Thanks to Common.NavigationHelper we can get the Puzzle 
        /// from NavigationParameter or PageState (in case of Tombstoned)
        /// </summary>
        /// <param name="e"></param>
        public void LoadState(LoadStateEventArgs e)
        {
            Debug.WriteLine("GameViewModel.LoadState");

            if (e.PageState != null && e.PageState.ContainsKey(PiecesViewModelPropertyName))
            {
                PiecesViewModel = JsonConvert.DeserializeObject<PiecesViewModel>(e.PageState[PiecesViewModelPropertyName].ToString());
                Name = e.PageState[NamePropertyName].ToString();
                Time = (int)e.PageState[TimePropertyName];
                _bestTime = (int)e.PageState[BestTimePropertyName];
                dispatcherTime.Start();
            }
            else
            {
                Puzzle puzzle = JsonConvert.DeserializeObject<Puzzle>(e.NavigationParameter.ToString());
                PiecesViewModel = new PiecesViewModel(puzzle.Pieces);
                Name = puzzle.Name;
                _bestTime = puzzle.BestTime;
                
                initGame();                 
            }
        }

        /// <summary>
        /// SaveState invoked when Tombstoned
        /// </summary>
        /// <param name="e"></param>
        public void SaveState(SaveStateEventArgs e)
        {
            Debug.WriteLine("GameViewModel.SaveState");
            e.PageState[PiecesViewModelPropertyName] = JsonConvert.SerializeObject(PiecesViewModel);
            //Debug.WriteLine(JsonConvert.SerializeObject(PiecesViewModel));
            e.PageState[NamePropertyName] = Name;
            e.PageState[TimePropertyName] = Time;
            e.PageState[BestTimePropertyName] = _bestTime;            
        }

        private void dispatcherTimeTick(object sender, object e)
        {
            Time++;            
        }

        private void initGame()
        {
            Time = 0;            
            dispatcherTime.Start();
        }

        private void finishGame(NotificationMessageAction notificationMessageAction)
        {
            if (notificationMessageAction.Notification.Equals("win"))
            {
                dispatcherTime.Stop();
                if (Time < _bestTime || _bestTime == 0)
                {
                    _bestTime = Time;
                    ApplicationData.Current.LocalSettings.Values[Name] = Time;
                }
                _dialogService.ShowMessage(
                    "Best time " + HelpConvert.intToStringTime(_bestTime) + "\nCurrent time " + HelpConvert.intToStringTime(Time),
                    "Puzzle Completed",
                    "Menu",
                    "Retry",
                    (menu) =>
                    {
                        if (menu)
                        {
                            _navigationService.GoBack();
                        }
                        else
                        {
                            initGame();
                            notificationMessageAction.Execute(); // execute the callback = restart
                        }
                    });
            }            
        }

    }
}
