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

        public GameViewModel(Puzzle model)
        {
            Model = model;
            Pieces = model.Pieces;
            for (int i = 0; i < Pieces.Count; i++)
            {
                Pieces[i].IndexTag = i;
                Pieces[i].Style = "PolygonNormal";
            }

            PolygonManipulationDeltaCommand = new RelayCommand<ManipulationDeltaRoutedEventArgs>(this.ExecutePolygonManipulationDelta);
            PolygonTappedCommand = new RelayCommand<TappedRoutedEventArgs>(this.ExecutePolygonTapped);
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

        public RelayCommand<ManipulationDeltaRoutedEventArgs> PolygonManipulationDeltaCommand { get; private set; }
        public void ExecutePolygonManipulationDelta(ManipulationDeltaRoutedEventArgs parameter)
        {
            Polygon polygon = parameter.OriginalSource as Polygon;
                        
            Pieces[int.Parse(polygon.Tag.ToString())].Left += parameter.Delta.Translation.X;
            Pieces[int.Parse(polygon.Tag.ToString())].Top += parameter.Delta.Translation.Y;
        }

        public RelayCommand<TappedRoutedEventArgs> PolygonTappedCommand { get; private set; }
        public void ExecutePolygonTapped(TappedRoutedEventArgs parameter)
        {
            Polygon polygon = parameter.OriginalSource as Polygon;

            Pieces[int.Parse(polygon.Tag.ToString())].Angle += 45;            
        }

    }
}
