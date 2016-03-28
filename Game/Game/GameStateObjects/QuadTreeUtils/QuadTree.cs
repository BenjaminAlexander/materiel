using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public delegate Vector2 GetTreePosition(PhysicalObject obj);

    public class QuadTree
    {
        private Vector2 mapSize;
        private Node root;
        private GetTreePosition positionFunc;

        LeafDictionary leafDictionary;

        public QuadTree(Vector2 mapSize, GetTreePosition positionFunc)
        {
            this.mapSize = mapSize;
            this.positionFunc = positionFunc;
            leafDictionary = new LeafDictionary(this);
            Rectangle mapRectangle = new Rectangle(0, 0, (int)Math.Ceiling(mapSize.X), (int)Math.Ceiling(mapSize.Y));
            root = new InternalNode(true, null, mapRectangle, leafDictionary, this.positionFunc);
        }

        public bool Add(PhysicalObject unit)
        {
            if(root.Add(unit))
            {
                return true;
            }
            else
            {
                //throw new Exception("add failed");
                return false;
            }
        }

        public List<PhysicalObject> GetObjectsInCircle(Vector2 center, float radius)
        {
            return root.GetObjectsInCircle(center, radius);
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

    }
}
