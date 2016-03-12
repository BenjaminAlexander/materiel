using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.IO.Events
{
    class RightMousePressed : IOEvent
    {
        public override bool hasOccured()
        {
            return IOState.rightButtonPressed();
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType();
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
