using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using QiQiaoBan.Design;
using QiQiaoBan.Helpers;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace QiQiaoBan.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        /*
         * Constructor
         */
        public MainViewModel(IDataService dataService, INavigationService navigationService)
        {
            Title = "QiQiaoBan";
            _dataService = dataService;
            _navigationService = navigationService;
            Puzzles = new ObservableCollection<GameViewModel>();
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            private set
            {
                Set("Title", ref _title, value);//_title = value;
            }
        }

        public ObservableCollection<GameViewModel> Puzzles
        {
            get;
            private set;
        }

        private RelayCommand _buttonRefreshCommand;
        public RelayCommand ButtonRefreshCommand
        {
            get
            {
                return _buttonRefreshCommand ?? (_buttonRefreshCommand = new RelayCommand(GetPuzzles));
            }
        }

        private async void GetPuzzles()
        {
            var puzzles = await _dataService.GetPuzzlesLocal();
            if (puzzles != null)
            {
                Puzzles.Clear();
                foreach (var puzzle in puzzles)
                {
                    Puzzles.Add(new GameViewModel(puzzle));
                }
            }
            Debug.WriteLine("GetPuzzles()");
        }

        public const string SelectedPuzzlePropertyName = "SelectedPuzzle";
        private GameViewModel _selectedPuzzle;
        public GameViewModel SelectedPuzzle
        {
            get
            {
                return _selectedPuzzle;
            }
            set
            {
                if (Set(SelectedPuzzlePropertyName, ref _selectedPuzzle, value) && value != null)
                {
                    _navigationService.Navigate(typeof(GamePage));
                }
            }
        }
    }
}
