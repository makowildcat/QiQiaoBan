using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml.Input;

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
            this.PolygonManipulationDeltaCommand = new RelayCommand<ManipulationDeltaRoutedEventArgs>(this.ExecuteCommand);
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
        
        public void ExecuteCommand(ManipulationDeltaRoutedEventArgs parameter)
        {
            Windows.UI.Xaml.Shapes.Rectangle rect = parameter.OriginalSource as Windows.UI.Xaml.Shapes.Rectangle;
            
            //Pieces[int.Parse(rect.Tag.ToString())].Left += parameter.Delta.Translation.X;
            //Pieces[int.Parse(rect.Tag.ToString())].Top += parameter.Delta.Translation.Y;
            Pieces[0].Left += parameter.Delta.Translation.X;
            Pieces[0].Top += parameter.Delta.Translation.Y;
        }
    }
}
