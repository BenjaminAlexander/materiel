using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    abstract class QuadTreeSearch<T> where T : PhysicalObject
    {
        private GameObjectField.Modes mode;

        protected GameObjectField.Modes Mode
        {
            get
            {
                return mode;
            }
        }

        public QuadTreeSearch(GameObjectField.Modes mode)
        {
            this.mode = mode;
        }

        public abstract bool SelectNode(Node node);

        public abstract void ExamineObject(T obj);
    }
}
