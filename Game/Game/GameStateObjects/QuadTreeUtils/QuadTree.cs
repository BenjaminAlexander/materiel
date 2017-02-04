using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public delegate bool Select<T>(T obj) where T : PhysicalObject;

    public class QuadTree
    {
        private Vector2 mapSize;
        private Node root;
        private GameObjectField.Modes mode;

        LeafDictionary leafDictionary;

        public QuadTree(Vector2 mapSize, GameObjectField.Modes mode)
        {
            this.mapSize = mapSize;
            this.mode = mode;
            leafDictionary = new LeafDictionary();
            Rectangle mapRectangle = new Rectangle(0, 0, (int)Math.Ceiling(mapSize.X), (int)Math.Ceiling(mapSize.Y));
            root = new InternalNode(null, mapRectangle, leafDictionary, this.mode);
        }

        public bool Add(PhysicalObject unit)
        {
            if (root.Add(unit))
            {
                return true;
            }
            else
            {
                //throw new Exception("add failed");
                return false;
            }
        }

        public List<T> GetObjectsInCircle<T>(Vector2 center, float radius) where T : PhysicalObject
        {
            CircleSearch<T> searchObj = new CircleSearch<T>(this.mode, center, radius);
            root.SearchNode(searchObj);
            return searchObj.List;
        }

        public List<T> GetObjects<T>(Vector2 center) where T : PhysicalObject
        {
            PointIntersectionSearch<T> searchObj = new PointIntersectionSearch<T>(this.mode, center);
            root.SearchNode(searchObj);
            return searchObj.List;
        }

        public T GetClosest<T>(Vector2 point, Select<T> selectFunc) where T : PhysicalObject
        {
            ClosestSearch<T> searchObj = new ClosestSearch<T>(this.mode, point, selectFunc);
            root.SearchNode(searchObj);
            return searchObj.Closest;
        }

        public bool Remove(PhysicalObject unit)
        {
            return root.Remove(unit);
        }

        public List<PhysicalObject> CompleteList()
        {
            return root.CompleteList();
        }

        public void Move(PhysicalObject obj)
        {
            leafDictionary.GetLeaf(obj).Move(obj);
        }

        internal Node Root
        {
            get { return root; }
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            root.Draw(gameTime, graphics);
        }
    }
}
