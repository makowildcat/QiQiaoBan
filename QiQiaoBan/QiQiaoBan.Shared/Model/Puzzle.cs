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

        public const string BestTimePropertyName = "BestTime";
        private int _bestTime;
        public int BestTime
        {
            get
            {
                return _bestTime;
            }
            set
            {
                Set(BestTimePropertyName, ref _bestTime, value);
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
    }
}
