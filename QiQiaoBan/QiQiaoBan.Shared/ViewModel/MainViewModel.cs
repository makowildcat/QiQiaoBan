using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private string _title;
        public string Title {
            get
            {
                return _title;
            }
            private set
            {
                _title = value;
            }
        }

        public MainViewModel()
        {
            Title = "QiQiaoBan";
        }
    }
}
