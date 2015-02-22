using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QiQiaoBan.ViewModel
{
    public interface IViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        void LoadState();
        void SaveState();
    }
}
