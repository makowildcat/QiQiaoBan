using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace QiQiaoBan.Model
{
    public class Piece : ObservableObject
    {
        public const string LeftPropertyName = "Left";
        private double _left;
        [XmlAttribute]
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
        [XmlAttribute]
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
        [XmlAttribute]
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
        
        public const string IndexTagPropertyName = "IndexTag";
        private int _indexTag;
        [XmlIgnore]
        public int IndexTag
        {
            get
            {
                return _indexTag;
            }
            set
            {
                Set(IndexTagPropertyName, ref _indexTag, value);
            }
        }

        public const string StylePropertyName = "Style";
        private string _style;
        [XmlIgnore]
        public string Style
        {
            get
            {
                return _style;
            }
            set
            {
                Set(StylePropertyName, ref _style, value);
            }
        }

        public const string ZIndexPropertyName = "ZIndex";
        private int _zindex;
        [XmlIgnore]
        public int ZIndex
        {
            get
            {
                return _zindex;
            }
            set
            {
                Set(ZIndexPropertyName, ref _zindex, value);
            }
        }

        private string _type;
        [XmlAttribute]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public const string PointsPropertyName = "Points";
        [XmlIgnore]
        public PointCollection Points
        {
            get
            {
                if (Type.Equals(Piece.SQUARE))
                    return square();
                if (Type.Equals(Piece.PARALLELOGRAM))
                    return parallelogram();
                if (Type.Equals(Piece.SMALLTRIANGLE))
                    return smallTriangle();
                if (Type.Equals(Piece.MEDIUMTRIANGLE))
                    return mediumTriangle();
                if (Type.Equals(Piece.LARGETRIANGLE))
                    return largeTriangle();
                return null;
            }
        }

        public Piece() { }

        public static string SQUARE = "square";
        public static PointCollection square()
        {
            return new PointCollection() { new Point(-50, 0), new Point(0, 50), new Point(50, 0), new Point(0, -50) };
        }

        public static string PARALLELOGRAM = "parallelogram";
        public static PointCollection parallelogram()
        {
            return new PointCollection() { new Point(-25, -25), new Point(-25, 75), new Point(25, 25), new Point(25, -75) };
        }

        public static string SMALLTRIANGLE = "smallTriangle";
        public static PointCollection smallTriangle()
        {
            return new PointCollection() { new Point(0, -50), new Point(0, 50), new Point(50, 0) };
        }

        public static string MEDIUMTRIANGLE = "mediumTriangle";
        public static PointCollection mediumTriangle()
        {
            return new PointCollection() { new Point(-50, -50), new Point(50, 50), new Point(50, -50) };
        }

        public static string LARGETRIANGLE = "largeTriangle";
        public static PointCollection largeTriangle()
        {
            return new PointCollection() { new Point(0, -100), new Point(0, 100), new Point(100, 0) };
        }
    }
}
