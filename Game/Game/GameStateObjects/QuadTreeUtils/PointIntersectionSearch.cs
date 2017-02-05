using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class PointIntersectionSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private Vector2 point;
        private List<T> rtn = new List<T>();

        public List<T> List
        {
            get
            {
                return rtn;
            }
        }

        public PointIntersectionSearch(GameObjectField.Modes mode, Vector2 point) : base(mode)
        {
            this.point = point;
        }

        public override void ExamineObject(T obj)
        {
            if (obj.CollidesWith(point, this.Mode))
            {
                rtn.Add(obj);
            }
        }

        public override bool SelectNode(Node node)
        {
            if (node.Bounds != null)
            {
                Rectangle bound = (Rectangle)node.Bounds;
                return bound.Contains(point);
            }
            return false;
        }
    }
}
