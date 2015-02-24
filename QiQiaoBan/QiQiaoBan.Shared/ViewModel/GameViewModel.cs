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
    public class GameViewModel : ViewModelBase, IViewModel
    {
        private const double DELTA_MARGIN = 20.0;
        private const int TIME_INTERVAL_SECOND = 1;

        private IDialogService _dialogService = null;
        private GalaSoft.MvvmLight.Views.INavigationService _navigationService = null;

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

            PolygonManipulationStartedCommand = new RelayCommand<ManipulationStartedRoutedEventArgs>(ExecutePolygonManipulationStarted);
            PolygonManipulationDeltaCommand = new RelayCommand<ManipulationDeltaRoutedEventArgs>(ExecutePolygonManipulationDelta);
            PolygonManipulationCompletedCommand = new RelayCommand<ManipulationCompletedRoutedEventArgs>(ExecutePolygonManipulationCompleted);
            PolygonTappedCommand = new RelayCommand<TappedRoutedEventArgs>(ExecutePolygonTapped);
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

        /// <summary>
        /// LoadState invoked when Tombstoned
        /// </summary>
        /// <param name="e"></param>
        public void SaveState(SaveStateEventArgs e)
        {
            Debug.WriteLine("GameViewModel.SaveState");
            e.PageState["Model"] = JsonConvert.SerializeObject(Model);
            e.PageState["ZIndex"] = ZIndex;
            e.PageState["countMatched"] = countMatched;
            e.PageState["Time"] = Time;
            e.PageState["indexDivider"] = indexDivider;
        }

        /*
         * (Polygon) Piece's manipulation (Started, Delta, Completed & Tapped)
         */
        /// <summary>
        /// Executed when a piece starts to move
        /// </summary>
        /// <param name="parameter"></param>
        public void ExecutePolygonManipulationStarted(ManipulationStartedRoutedEventArgs parameter)
        {
            // !Trick: Get index from Tag property
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;
            var piece = Pieces[index];

            piece.ZIndex = ++ZIndex;
            if (piece.MatchWithIndex >= 0)
                UnMatch(piece);
        }

        public void ExecutePolygonManipulationDelta(ManipulationDeltaRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;
            var piece = Pieces[index];

            piece.Left += parameter.Delta.Translation.X;
            piece.Top += parameter.Delta.Translation.Y;
        }

        public void ExecutePolygonManipulationCompleted(ManipulationCompletedRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;
            var piece = Pieces[index];

            checkAllForMatching(piece);
        }

        public void ExecutePolygonTapped(TappedRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;
            var piece = Pieces[index];

            piece.ZIndex = ++ZIndex;
            if (piece.MatchWithIndex >= 0)
                UnMatch(piece);
            piece.Angle = Rotate(piece.Angle);
            checkAllForMatching(piece);
        }

        /// <summary>
        /// We check if the moving piece match with one of the locking piece
        /// </summary>
        /// <param name="piece"></param>
        private void checkAllForMatching(Piece piece)
        {
            for (int i = 0; i < indexDivider; i++)
            {
                if (Pieces[i].MatchWithIndex >= 0)
                    continue;

                if (isMatching(piece, Pieces[i]))
                {
                    Match(piece, Pieces[i]);                    
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

        // Add 45° and never more than 360 (easier to check if angle match)
        private double Rotate(double currentAngle)
        {
            return (currentAngle+45) % 360;
        }

        private int FrameworkElementTagToInt(FrameworkElement frameworkElement)
        {
            return int.Parse(frameworkElement.Tag.ToString());
        }

        /// <summary>
        /// Because Square and Parallelogram have the same rotation every 90° and 180°
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
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

        /// <summary>
        /// If some Piece are matching then we move them (like magnet)
        /// and we save the Id of each other, also change style, etc
        /// </summary>
        /// <param name="pieceMoving"></param>
        /// <param name="pieceLock"></param>
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
