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
using Windows.UI.Xaml.Navigation;

namespace QiQiaoBan.ViewModel
{
    class MainViewModel : ViewModelBase, IViewModel
    {
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            private set
            {
                Set("Title", ref _title, value);
            }
        }

        public ObservableCollection<Puzzle> Puzzles
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
                    Puzzles.Add(puzzle);//new GameViewModel());
                }
            }
            Debug.WriteLine("GetPuzzles()");
        }

        public const string SelectedPuzzlePropertyName = "SelectedPuzzle";
        private Puzzle _selectedPuzzle;
        public Puzzle SelectedPuzzle
        {
            get
            {
                return _selectedPuzzle;
            }
            set
            {
                if (Set(SelectedPuzzlePropertyName, ref _selectedPuzzle, value) && value != null)
                {
                    Debug.WriteLine("SelectedPuzzle.Pieces.Count > " + _selectedPuzzle.Pieces.Count);
                    _navigationService.Navigate(typeof(GamePage), value);
                }
            }
        }

        public MainViewModel(IDataService dataService, INavigationService navigationService)
        {
            Title = "QiQiaoBan";
            _dataService = dataService;
            _navigationService = navigationService;
            Puzzles = new ObservableCollection<Puzzle>();
        }

        public void NavigateTo(NavigationEventArgs e)
        {
            Debug.WriteLine("MainViewModel.NavigateTo");
        }

        public void NavigateFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("MainViewModel.NavigateFrom");
        }
    }
}
