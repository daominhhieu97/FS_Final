using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Classes
{
    class Player
    {
        String _name;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        internal Player Clone()
        {
            Player deepCopy = new Player();
            deepCopy.Name = this.Name;
            return deepCopy;
        }
    }
}
