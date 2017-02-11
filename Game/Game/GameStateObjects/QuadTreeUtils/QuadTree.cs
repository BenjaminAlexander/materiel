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

        public void SearchDownFromRoot<T>(QuadTreeSearch<T> searchObj) where T : PhysicalObject
        {
            root.SearchDown(searchObj);
        }

        public void SearchUpFromLeaf<T>(QuadTreeSearch<T> searchObj, PhysicalObject leafObject) where T : PhysicalObject
        {
            this.leafDictionary.GetLeaf(leafObject).SearchUP<T>(searchObj);
        }

        public bool Remove(PhysicalObject unit)
        {
            return root.Remove(unit);
        }

        public void Move(PhysicalObject obj)
        {
            leafDictionary.GetLeaf(obj).Move(obj);
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            root.Draw(gameTime, graphics);
        }
    }
}
