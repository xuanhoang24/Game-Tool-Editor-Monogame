using Editor.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GUI
{
    internal class ListItemLevel
    {
        public Models Model { get; set; }

        public override string ToString()
        {
            return Model.Name;
        }
    }
}
