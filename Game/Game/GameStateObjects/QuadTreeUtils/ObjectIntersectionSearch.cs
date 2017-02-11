using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class ObjectIntersectionSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private PhysicalObject obj;
        private Rectangle bound;
        private List<T> rtn = new List<T>();

        public static List<T> GetObjects(GameObjectCollection collection, PhysicalObject obj)
        {
            ObjectIntersectionSearch<T> searchObj = new ObjectIntersectionSearch<T>(GameObjectField.Mode, obj);
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

        public ObjectIntersectionSearch(GameObjectField.Modes mode, PhysicalObject obj)
        {
            this.obj = obj;
            this.bound = obj.GetBoundingRectangle(mode);
        }

        public override void ExamineObject(T other, GameObjectField.Modes mode)
        {
            if (other != this.obj && other.CollidesWith(this.obj, mode))
            {
                rtn.Add(other);
            }
        }

        public override bool SelectNode(Node node)
        {
            if (node.Bounds != null)
            {
                Rectangle nodeBound = (Rectangle)node.Bounds;
                return Utils.Vector2Utils.Intersects(nodeBound, bound);
            }
            return false;
        }

        public override bool SelectParentNode(Node currentNode)
        {
            return true;
        }
    }
}
