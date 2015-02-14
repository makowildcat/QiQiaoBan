using GalaSoft.MvvmLight;
using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

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
            foreach (var piece in Model.Pieces)
            {
                Debug.WriteLine(piece.Left + ":" + piece.Top);
            }
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        public List<Piece> Pieces
        {
            get
            {
                return Model.Pieces;
            }
        }
    }
}
