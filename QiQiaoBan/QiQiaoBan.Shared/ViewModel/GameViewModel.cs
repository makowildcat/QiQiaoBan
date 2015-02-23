using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    class GameViewModel : ViewModelBase, IViewModel
    {
        private const double DELTA_MARGIN = 20.0;
        private const int TIME_INTERVAL_SECOND = 1;

        private IDialogService _dialogService = null;
        private QiQiaoBan.Helpers.INavigationService _navigationService = null;

        public Puzzle Model
        {
            get;
            private set;
        }

        public const string PiecesPropertyName = "Pieces";
        private List<Piece> _pieces;
        public List<Piece> Pieces
        {
            get
            {
                return _pieces;
            }
            set
            {
                Set(PiecesPropertyName, ref _pieces, value);
            }
        }

        public RelayCommand<ManipulationStartedRoutedEventArgs> PolygonManipulationStartedCommand { get; private set; }
        public RelayCommand<ManipulationDeltaRoutedEventArgs> PolygonManipulationDeltaCommand { get; private set; }
        public RelayCommand<ManipulationCompletedRoutedEventArgs> PolygonManipulationCompletedCommand { get; private set; }
        public RelayCommand<TappedRoutedEventArgs> PolygonTappedCommand { get; private set; }

        private int ZIndex;
        private int indexDivider;
        private int countMatched;

        public string Name
        {
            get
            {
                return Model.Name;
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
        private DispatcherTimer dispatcherTime;
        
        public GameViewModel(QiQiaoBan.Helpers.INavigationService navigationService, IDialogService dialogService)
        {
            Debug.WriteLine("GameViewModel.constructor");

            _dialogService = dialogService;
            _navigationService = navigationService;

            dispatcherTime = new DispatcherTimer();
            dispatcherTime.Interval = TimeSpan.FromSeconds(TIME_INTERVAL_SECOND);
            dispatcherTime.Tick += dispatcherTimeTick;

            PolygonManipulationStartedCommand = new RelayCommand<ManipulationStartedRoutedEventArgs>(ExecutePolygonManipulationStarted);
            PolygonManipulationDeltaCommand = new RelayCommand<ManipulationDeltaRoutedEventArgs>(ExecutePolygonManipulationDelta);
            PolygonManipulationCompletedCommand = new RelayCommand<ManipulationCompletedRoutedEventArgs>(ExecutePolygonManipulationCompleted);
            PolygonTappedCommand = new RelayCommand<TappedRoutedEventArgs>(ExecutePolygonTapped);
        }

        public void LoadState(LoadStateEventArgs e)
        {
            Debug.WriteLine("GameViewModel.LoadState");

            if (e.PageState != null && e.PageState.ContainsKey("Model"))
            {
                Model = JsonConvert.DeserializeObject<Puzzle>(e.PageState["Model"].ToString());
                Pieces = Model.Pieces;

                ZIndex = (int)e.PageState["ZIndex"];
                countMatched = (int)e.PageState["countMatched"];
                Time = (int)e.PageState["Time"];
                indexDivider = (int)e.PageState["indexDivider"];
                dispatcherTime.Start();
            }
            else
            {
                Model = JsonConvert.DeserializeObject<Puzzle>(e.NavigationParameter.ToString());
                Pieces = Model.Pieces;
                indexDivider = Pieces.Count;
                for (int i = 0; i < indexDivider; i++)
                {
                    Pieces.Add(new Piece());
                }

                initGame();
            }
        }

        private void dispatcherTimeTick(object sender, object e)
        {
            Time++;            
        }

        private void initGame()
        {
            ZIndex = 0;
            countMatched = 0;
            Time = 0;
            
            Random random = new Random();
            for (int i = 0; i < indexDivider; i++)
            {
                Pieces[i].IndexTag = i;
                Pieces[i].ZIndex = -2;
                Pieces[i].Style = "PolygonLock";
                Pieces[i].MatchWithIndex = -1;

                Pieces[i + indexDivider].ZIndex = ++ZIndex;
                Pieces[i + indexDivider].Style = "PolygonNormal";
                Pieces[i + indexDivider].IndexTag = i + indexDivider;
                Pieces[i + indexDivider].Type = Pieces[i].Type;
                Pieces[i + indexDivider].Left = random.Next(10, 300);
                Pieces[i + indexDivider].Top = random.Next(10, 400);
                Pieces[i + indexDivider].Angle = random.Next(0, 8) * 45;
                Pieces[i + indexDivider].MatchWithIndex = -1;
            }

            dispatcherTime.Start();
        }

        public void SaveState(SaveStateEventArgs e)
        {
            Debug.WriteLine("GameViewModel.SaveState");
            e.PageState["Model"] = JsonConvert.SerializeObject(Model);
            e.PageState["ZIndex"] = ZIndex;
            e.PageState["countMatched"] = countMatched;
            e.PageState["Time"] = Time;
            e.PageState["indexDivider"] = indexDivider;
        }

        public void ExecutePolygonManipulationStarted(ManipulationStartedRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;

            Pieces[index].ZIndex = ++ZIndex;
            if (Pieces[index].MatchWithIndex >= 0)
                UnMatch(Pieces[index]);
        }

        public void ExecutePolygonManipulationDelta(ManipulationDeltaRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;

            Pieces[index].Left += parameter.Delta.Translation.X;
            Pieces[index].Top += parameter.Delta.Translation.Y;
        }

        public void ExecutePolygonManipulationCompleted(ManipulationCompletedRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;

            checkAllForMatching(index);
        }

        public void ExecutePolygonTapped(TappedRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;

            Pieces[index].ZIndex = ++ZIndex;
            if (Pieces[index].MatchWithIndex >= 0)
                UnMatch(Pieces[index]);
            Pieces[index].Angle = Rotate(Pieces[index].Angle);
            checkAllForMatching(index);
        }

        private void checkAllForMatching(int index)
        {
            for (int i = 0; i < indexDivider; i++)
            {
                if (Pieces[i].MatchWithIndex >= 0)
                    continue;

                if (isMatching(Pieces[index], Pieces[i]))
                {
                    Match(Pieces[index], Pieces[i]);                    
                }
            }

            if (countMatched == indexDivider)
                finishGame();
        }

        private void finishGame()
        {
            Debug.WriteLine("You win!!");
            dispatcherTime.Stop();
            if (Time < Model.BestTime || Model.BestTime == 0)
            {
                Model.BestTime = Time;
                ApplicationData.Current.LocalSettings.Values[Name] = Time;
            }
            _dialogService.ShowMessage(
                "Best time " + HelpConvert.intToStringTime(Model.BestTime) + "\nCurrent time " + HelpConvert.intToStringTime(Time), 
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
                    }
                });
        }

        private double Rotate(double currentAngle)
        {
            return (currentAngle+45) % 360;
        }

        private int FrameworkElementTagToInt(FrameworkElement polygon)
        {
            return int.Parse(polygon.Tag.ToString());
        }

        private double getAngleModulo(Piece piece)
        {
            if (piece.Type.Equals(Piece.SQUARE))
                return 90;
            if (piece.Type.Equals(Piece.PARALLELOGRAM))
                return 180;
            return 360;
        }

        private bool isSameType(Piece p1, Piece p2)
        {
            return p1.Type == p2.Type;
        }

        private bool isSameAngle(Piece p1, Piece p2)
        {
            return p1.Angle % getAngleModulo(p1) == p2.Angle % getAngleModulo(p2);
        }

        private bool isSameCoord(Piece p1, Piece p2, double margin = 0.0)
        {
            var deltaLeft = p1.Left - p2.Left;
            var deltaTop = p1.Top - p2.Top;
            return (deltaLeft >= -margin && deltaLeft <= margin) && (deltaTop >= -margin && deltaTop <= margin);
        }

        private bool isMatching(Piece p1, Piece p2)
        {
            if (!isSameType(p1, p2))
                return false;

            if (!isSameAngle(p1, p2))
                return false;

            if (!isSameCoord(p1, p2, DELTA_MARGIN))
                return false;

            return true;
        }

        private void Match(Piece pieceMoving, Piece pieceLock)
        {
            countMatched++;
            pieceMoving.Left = pieceLock.Left;
            pieceMoving.Top = pieceLock.Top;
            pieceMoving.Style = "PolygonMatch";
            pieceMoving.ZIndex = -1;
            pieceMoving.MatchWithIndex = pieceLock.IndexTag;
            pieceLock.MatchWithIndex = pieceMoving.IndexTag;
        }

        private void UnMatch(Piece p1)
        {
            countMatched--;
            Pieces[p1.MatchWithIndex].MatchWithIndex = -1;
            p1.MatchWithIndex = -1;
            p1.Style = "PolygonNormal";
        }
    }
}
