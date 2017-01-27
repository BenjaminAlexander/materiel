﻿using System;
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
            return root.GetObjectsInCircle<T>(center, radius);
        }

        public List<T> GetObjects<T>(Vector2 center) where T : PhysicalObject
        {
            return root.GetObjects<T>(center);
        }

        public T GetClosest<T>(Vector2 point, Select<T> selectFunc) where T : PhysicalObject
        {
            return root.GetClosest<T>(point, selectFunc, null);
        }

        public bool Remove(PhysicalObject unit)
        {
            Leaf removeFrom = root.Remove(unit);
            if (removeFrom != null)
            {
                removeFrom.Collapse();
                return true;
            }
            else
            {
                //throw new Exception("No object to remove");
                return false;
            }
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
