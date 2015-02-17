using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace QiQiaoBan.ViewModel
{
    class GameViewModel : ViewModelBase
    {
        public Puzzle Model
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
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

        public GameViewModel(Puzzle model)
        {
            Model = model;
            Pieces = model.Pieces;

            ZIndex = 0;
            indexDivider = Pieces.Count;
            countMatched = 0;

            Random random = new Random();
            for (int i = 0; i < indexDivider; i++)
            {
                Pieces[i].IndexTag = i;
                Pieces[i].ZIndex = -2;
                Pieces[i].Style = "PolygonLock";
                Pieces[i].MatchWithIndex = -1;

                Pieces.Add(new Piece() { 
                    ZIndex = ++ZIndex, 
                    Style = "PolygonNormal", 
                    IndexTag = i + indexDivider, 
                    Type = Pieces[i].Type, 
                    Left = random.Next(10, 300), 
                    Top = random.Next(10, 400), 
                    Angle = random.Next(0, 8) * 45, 
                    MatchWithIndex = -1 
                });
            }

            PolygonManipulationStartedCommand = new RelayCommand<ManipulationStartedRoutedEventArgs>(ExecutePolygonManipulationStarted);
            PolygonManipulationDeltaCommand = new RelayCommand<ManipulationDeltaRoutedEventArgs>(ExecutePolygonManipulationDelta);
            PolygonManipulationCompletedCommand = new RelayCommand<ManipulationCompletedRoutedEventArgs>(ExecutePolygonManipulationCompleted);
            PolygonTappedCommand = new RelayCommand<TappedRoutedEventArgs>(ExecutePolygonTapped);
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
                Debug.WriteLine("You win!!");
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

            if (!isSameCoord(p1, p2, 20.0))
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
