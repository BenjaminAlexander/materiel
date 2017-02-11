using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class PointIntersectionSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private Vector2 point;
        private List<T> rtn = new List<T>();

        public static List<T> GetObjects(GameObjectCollection collection, Vector2 point)
        {
            PointIntersectionSearch<T> searchObj = new PointIntersectionSearch<T>(point);
            collection.SearchDownFromRoot<T>(searchObj);
            return searchObj.List;
        }

        public List<T> List
        {
            get
            {
                return rtn;
            }
        }

        public PointIntersectionSearch(Vector2 point)
        {
            this.point = point;
        }

        public override void ExamineObject(T obj, GameObjectField.Modes mode)
        {
            if (obj.CollidesWith(point, mode))
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

        public override bool SelectParentNode(Node currentNode)
        {
            return true;
        }
    }
}
