using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class ListAllSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private List<T> rtn;

        public ListAllSearch()
        {
            rtn = new List<T>();
        }

        public ListAllSearch(List<T> list)
        {
            rtn = list;
        }

        public List<T> List
        {
            get
            {
                return rtn;
            }
        }

        public override void ExamineObject(T obj)
        {
            rtn.Add(obj);
        }

        public override bool SelectNode(Node node)
        {
            return true;
        }
    }
}
