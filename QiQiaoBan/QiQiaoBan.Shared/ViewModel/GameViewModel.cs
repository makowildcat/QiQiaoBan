using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QiQiaoBan.ViewModel
{
    class GameViewModel
    {
        public Puzzle Model
        {
            get;
            private set;
        }

        public GameViewModel(Puzzle model)
        {
            Model = model;
        }

        public string Name
        {
            get
            {
                return Model.Name;
            }
        }
    }
}
