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

        public GameViewModel(Puzzle model)
        {
            Model = model;
            Pieces = model.Pieces;
            ZIndex = 0;
            indexDivider = Pieces.Count;

            Random random = new Random();
            for (int i = 0; i < indexDivider; i++)
            {
                Pieces[i].IndexTag = i;
                Pieces[i].ZIndex = -1;
                Pieces[i].Style = "PolygonLock";

                Pieces.Add(new Piece() { ZIndex = ++ZIndex, Style = "PolygonNormal", IndexTag = i + indexDivider, Type = Pieces[i].Type, Left = random.Next(10, 300), Top = random.Next(10, 400), Angle = random.Next(0, 8) * 45 });
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
            
            for (int i = 0; i < indexDivider; i++)
            {
                if (isMatching(Pieces[index], Pieces[i]))
                    Debug.WriteLine("Matched!!");
            }
        }

        public void ExecutePolygonTapped(TappedRoutedEventArgs parameter)
        {
            var index = FrameworkElementTagToInt(parameter.OriginalSource as FrameworkElement);
            if (index < indexDivider)
                return;

            Pieces[index].ZIndex = ++ZIndex;
            Pieces[index].Angle = Rotate(Pieces[index].Angle);
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

        private bool isMatching(Piece pieceMoving, Piece pieceLocked)
        {
            if (pieceMoving.Type != pieceLocked.Type)
                return false;
            Debug.WriteLine("Type matched");

            if (pieceMoving.Angle % getAngleModulo(pieceMoving) != pieceLocked.Angle % getAngleModulo(pieceLocked))
                return false;
            Debug.WriteLine("Angle matched");

            var deltaLeft = pieceMoving.Left - pieceLocked.Left;
            var deltaTop = pieceMoving.Top - pieceLocked.Top;
            var deltaMargin = 20;
            if (deltaLeft < -deltaMargin || deltaLeft > deltaMargin || deltaTop < -deltaMargin || deltaTop > deltaMargin)
                return false;
            Debug.WriteLine("Coord matched");

            return true;
        }
    }
}
