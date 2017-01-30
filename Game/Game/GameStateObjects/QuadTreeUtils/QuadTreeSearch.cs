using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    abstract class QuadTreeSearch<T> where T : PhysicalObject
    {
        public abstract bool SelectNode(Node node);

        public abstract void ExamineObject(T obj);
    }
}
