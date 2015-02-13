using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QiQiaoBan.Model
{
    public class Puzzle : ObservableObject
    {
        public const string NamePropertyName = "Name";
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(NamePropertyName, ref _name, value);
            }
        }

        public const string PiecesPropertyName = "Pieces";
        public ObservableCollection<Piece> Pieces;        
    }
}
