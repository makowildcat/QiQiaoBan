using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using QiQiaoBan.Common;
using QiQiaoBan.Design;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
    public class MainViewModel : ViewModelBase, IViewModel
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
                    _navigationService.NavigateTo(ViewModelLocator.GAME_PAGEKEY, JsonConvert.SerializeObject(value));                    
                }
            }
        }

        public MainViewModel(IDataService dataService, INavigationService navigationService)
        {
            Title = "QiQiaoBan";
            _dataService = dataService;
            _navigationService = navigationService;
            
            if (IsInDesignMode)
                DesignLoadPuzzles();
            else
                Puzzles = new ObservableCollection<Puzzle>();
        }

        private async void DesignLoadPuzzles()
        {
            Puzzles = new ObservableCollection<Puzzle>(await _dataService.GetPuzzlesLocal());
        }

        public async void LoadState(LoadStateEventArgs e)
        {
            Debug.WriteLine("MainViewModel.LoadState");
            var puzzles = await _dataService.GetPuzzlesLocal();
            if (puzzles != null)
            {
                Puzzles.Clear();
                foreach (var puzzle in puzzles)
                {
                    var localSettings = ApplicationData.Current.LocalSettings;
                    puzzle.BestTime = localSettings.Values.ContainsKey(puzzle.Name) ? (int)localSettings.Values[puzzle.Name] : 0;
                    Puzzles.Add(puzzle);
                }
            }
        }

        public void SaveState(SaveStateEventArgs e)
        {
            Debug.WriteLine("MainViewModel.SaveState");
        }
    }
}
