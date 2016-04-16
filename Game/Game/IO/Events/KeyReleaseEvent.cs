using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MyGame.IO.Events
{
    public class KeyReleaseEvent : IOEvent
    {
        Keys key;

        public KeyReleaseEvent(Keys key)
        {
            this.key = key;
        }

        public override bool hasOccured()
        {
            return IOState.isKeyReleased(key);
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType() && ((KeyReleaseEvent)obj).key == this.key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }
    }
}