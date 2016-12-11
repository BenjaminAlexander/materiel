using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    abstract class Node
    {
        protected static int max_count = 10;
        
        //private static int treeDepth = 10;
        private InternalNode parent;
        public int id;
        public static int nextI = 0;
        protected LeafDictionary leafDictionary;

        private Rectangle mapSpace;
        private GameObjectField.Modes mode;
        public Rectangle MapSpace
        {
            get { return mapSpace; }
        }

        public Node(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary, GameObjectField.Modes mode)
        {
            id = nextI++;
            this.parent = parent;
            this.leafDictionary = leafDictionary;
            this.mapSpace = mapSpace;
            this.mode = mode;
        }

        protected Vector2 GetObjPosition(PhysicalObject obj)
        {
            return obj.GetPosition(this.mode);
        }

        protected GameObjectField.Modes Mode
        {
            get
            {
                return this.mode;
            }
        }

        public abstract int ObjectCount();

        protected InternalNode Parent
        {
            get { return parent; }
        }

        public void DisconnectFromParent()
        {
            parent = null;
        }

        public abstract bool Add(PhysicalObject unit);

        public abstract Leaf Remove(PhysicalObject unit);

        public abstract bool Contains(Vector2 point);

        public static bool Contains(Rectangle rectangle, Vector2 point)
        {
            return rectangle.X <= point.X &&
                point.X < rectangle.X + rectangle.Width &&
                rectangle.Y <= point.Y &&
                point.Y < rectangle.Y + rectangle.Height;
        }

        public abstract List<T> GetObjectsInCircle<T>(Vector2 center, float radius, List<T> list) where T : PhysicalObject;

        public abstract List<T> CompleteList<T>(ref List<T> list) where T : PhysicalObject;

        public List<PhysicalObject> CompleteList()
        {
            List<PhysicalObject> list = new List<PhysicalObject>();
            return this.CompleteList(ref list);
        }

        public List<T> GetObjectsInCircle<T>(Vector2 center, float radius) where T : PhysicalObject
        {
            List<T> list = new List<T>();
            return this.GetObjectsInCircle(center, radius, list);
        }

        public abstract void Move(PhysicalObject obj);
    }
}
