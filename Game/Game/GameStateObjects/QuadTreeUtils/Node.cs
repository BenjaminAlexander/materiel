using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.PhysicalObjects;

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
        public Rectangle MapSpace
        {
            get { return mapSpace; }
        }

        public Node(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary)
        {
            id = nextI++;
            this.parent = parent;
            this.leafDictionary = leafDictionary;
            this.mapSpace = mapSpace;
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

        public abstract List<PhysicalObject> GetObjectsInCircle(Vector2 center, float radius, List<PhysicalObject> list);

        public abstract List<PhysicalObject> CompleteList(ref List<PhysicalObject> list);

        public List<PhysicalObject> CompleteList()
        {
            List<PhysicalObject> list = new List<PhysicalObject>();
            return this.CompleteList(ref list);
        }

        public List<PhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            List<PhysicalObject> list = new List<PhysicalObject>();
            return this.GetObjectsInCircle(center, radius, list);
        }

        public abstract void Move(PhysicalObject obj);
    }
}
