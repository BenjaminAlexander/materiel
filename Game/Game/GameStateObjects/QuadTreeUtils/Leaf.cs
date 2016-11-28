using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class Leaf : Node
    {
        private GameObjectListManager unitList;
        private GetTreePosition positionFunc;

        public override int ObjectCount()
        {
            return unitList.GetMaster().Count;
        }

        public Leaf(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary, GetTreePosition positionFunc)
            : base(parent, mapSpace, leafDictionary, positionFunc)
        {
            unitList = new GameObjectListManager();
        }

        public override bool Add(PhysicalObject unit)
        {
            if (this.Contains(this.GetObjPosition(unit)))
            {
                unitList.Add(unit);
                leafDictionary.SetLeaf(unit, this);

                if (ObjectCount() > max_count)
                {
                    this.Expand();
                }
                return true;
            }
            return false;
        }

        public override Leaf Remove(PhysicalObject unit)
        {
            if (unitList.Contains(unit))
            {
                leafDictionary.SetLeaf(unit, null);
                unitList.Remove(unit);
                return this;
            }
            return null;
        }

        public void Collapse()
        {
            this.Parent.Collapse();
        }

        public override bool Contains(Vector2 point)
        {
            return Node.Contains(MapSpace, point);
        }

        public override List<T> GetObjectsInCircle<T>(Vector2 center, float radius, List<T> list)
        {
            if (ObjectCount() > 0)
            {
                Vector2 rectangleCenter = new Vector2((((float)MapSpace.X) + ((float)MapSpace.Width) / 2), (((float)MapSpace.Y) + ((float)MapSpace.Height) / 2));
                float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(MapSpace.X, MapSpace.Y));

                if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                {

                    foreach (PhysicalObject unit in unitList.GetList<PhysicalObject>())
                    {
                        if (unit is T && Vector2.Distance(this.GetObjPosition(unit), center) <= radius)
                        {
                            list.Add((T)unit);
                        }
                    }
                }
            }
            return list;
        }

        public override List<T> CompleteList<T>(ref List<T> list)
        {
            foreach (PhysicalObject obj in unitList.GetList<PhysicalObject>())
            {
                if (obj is T)
                {
                    list.Add((T)obj);
                }
            }
            return list;
        }

        public override void Move(PhysicalObject obj)
        {
            if(unitList.Contains(obj))
            {
                if (!this.Contains(this.GetObjPosition(obj)))
                {
                    this.Remove(obj);
                    this.Parent.Move(obj);
                    if (unitList.Contains(obj))
                    {
                        throw new Exception("Move failed");
                    }
                    if (!this.Parent.IsChild(this))
                    {
                        throw new Exception("incorrect child/parent");
                    }
                    this.Parent.Collapse();
                }
            }
            else
            {
                throw new Exception("No such object");
            }
        }

        private void Expand()
        {
            if (MapSpace.Width > 1 && MapSpace.Height > 1)
            {
                Node newNode = new InternalNode(false, this.Parent, this.MapSpace, leafDictionary, this.PositionFunc);// (this.Parent, this.mapSpace, 2);
                this.Parent.Replace(this, newNode);
                foreach (PhysicalObject obj in unitList.GetList<PhysicalObject>())
                {
                    this.Remove(obj);
                    if (!newNode.Add(obj))
                    {
                        this.Parent.Move(obj);
                        //throw new Exception("Failed to add after move");
                    }
                }

                leafDictionary.DestroyLeaf(this);
            }
        }

        public Boolean Contains(PhysicalObject obj)
        {
            return this.unitList.Contains(obj);
        }
    }
}
