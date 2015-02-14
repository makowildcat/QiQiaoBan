using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.Model
{
    public class Piece : ObservableObject
    {
        public const string LeftPropertyName = "Left";
        private double _left;
        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                Set(LeftPropertyName, ref _left, value);
            }            
        }

        public const string TopPropertyName = "Top";
        private double _top;
        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                Set(TopPropertyName, ref _top, value);
            }
        }

        public const string AnglePropertyName = "Angle";
        private double _angle;
        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                Set(AnglePropertyName, ref _angle, value);
            }
        }

        public Piece() { }
    }
}
