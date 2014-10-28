using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD_Engine
{
    public class TowerEventArgs : EventArgs
    {
        public Tower _tower;
        public TowerEventArgs(Tower t)
        {
            _tower = t;
        }
    }
}
