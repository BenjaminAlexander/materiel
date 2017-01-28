using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    abstract class Node
    {
        protected static int max_count = 10;

        private InternalNode parent;
        public int id;
        public static int nextI = 0;
        protected LeafDictionary leafDictionary;

        private Rectangle mapSpace;
        private GameObjectField.Modes mode;
        private Rectangle objectBounds;

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

        public InternalNode Parent
        {
            get { return parent; }
            set
            {
                this.parent = value;
            }
        }

        public abstract bool Add(PhysicalObject unit);

        public abstract Leaf Remove(PhysicalObject unit);

        public bool Contains(Vector2 point)
        {
            return this.mapSpace.X <= point.X &&
                point.X < this.mapSpace.X + this.mapSpace.Width &&
                this.mapSpace.Y <= point.Y &&
                point.Y < this.mapSpace.Y + this.mapSpace.Height;
        }

        public abstract List<T> GetObjectsInCircle<T>(Vector2 center, float radius, List<T> list) where T : PhysicalObject;

        public abstract List<T> GetObjects<T>(Vector2 point, List<T> list) where T : PhysicalObject;

        public abstract T GetClosest<T>(Vector2 point, Select<T> selectFunc, T best) where T : PhysicalObject;

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

        public List<T> GetObjects<T>(Vector2 point) where T : PhysicalObject
        {
            List<T> list = new List<T>();
            return this.GetObjects(point, list);
        }

        public abstract void Move(PhysicalObject obj);

        public virtual void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.DrawWorldRectangleOnScreen(this.mapSpace, Color.Red, 1f);
        }

        public Rectangle Bounds
        {
            get
            {
                return objectBounds;
            }

            protected set
            {
                objectBounds = value;
            }
        }

        public float MinimumPositionDistance(Vector2 point)
        {
            return Utils.Vector2Utils.Distance(this.MapSpace, point);
        }
    }
}
