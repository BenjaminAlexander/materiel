using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    abstract class Node
    {
        protected static int max_count = 4;

        private InternalNode parent;
        public int id;
        public static int nextI = 0;
        protected LeafDictionary leafDictionary;

        private Rectangle mapSpace;
        private GameObjectField.Modes mode;
        private Rectangle? objectBounds;

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

        public GameObjectField.Modes Mode
        {
            get
            {
                return this.mode;
            }
        }

        public LeafDictionary LeafDictionary
        {
            get
            {
                return this.leafDictionary;
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

        public abstract bool Remove(PhysicalObject unit);

        public bool Contains(Vector2 point)
        {
            return this.mapSpace.X <= point.X &&
                point.X < this.mapSpace.X + this.mapSpace.Width &&
                this.mapSpace.Y <= point.Y &&
                point.Y < this.mapSpace.Y + this.mapSpace.Height;
        }

        public List<T> CompleteList<T>(List<T> list) where T : PhysicalObject
        {
            ListAllSearch<T> searchObj = new ListAllSearch<T>(this.Mode, list);
            this.SearchDown(searchObj);
            return searchObj.List;
        }

        public List<PhysicalObject> CompleteList()
        {
            ListAllSearch<PhysicalObject> searchObj = new ListAllSearch<PhysicalObject>(this.Mode);
            this.SearchDown(searchObj);
            return searchObj.List;
        }

        public virtual void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.DrawWorldRectangleOnScreen(this.mapSpace, Color.Red, 1f);
        }

        public Rectangle? Bounds
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

        //public abstract void SearchNode<T>(QuadTreeSearch<T> searchObj) where T : PhysicalObject;

        public abstract void SearchDown<T>(QuadTreeSearch<T> searchObj) where T : PhysicalObject;
    }
}
