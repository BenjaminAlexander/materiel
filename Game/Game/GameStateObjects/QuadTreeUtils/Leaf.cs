using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class Leaf : Node
    {
        private GameObjectListManager unitList;
        

        public override int ObjectCount()
        {
            return unitList.GetMaster().Count;
        }

        public Leaf(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary, GameObjectField.Modes mode)
            : base(parent, mapSpace, leafDictionary, mode)
        {
            unitList = new GameObjectListManager();
            this.Bounds = new Rectangle(mapSpace.X, mapSpace.Y, 0, 0);
        }

        public Rectangle ComputeBounds()
        {
            List<PhysicalObject> units = unitList.GetList<PhysicalObject>();
            if (units.Count > 0)
            {
                Rectangle bounds = units[0].BoundingRectangle;
                foreach(PhysicalObject obj in units)
                {
                    bounds = Utils.MathUtils.RectangleUnion(bounds, obj.BoundingRectangle);
                }
                this.Bounds = bounds;
            }
            else
            {
                this.Bounds = new Rectangle(this.MapSpace.X, this.MapSpace.Y, 0, 0);
            }
            this.Parent.ComputeBounds();
            return this.Bounds;
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
                this.ComputeBounds();
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
                this.ComputeBounds();
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
                Node newNode = new InternalNode(false, this.Parent, this.MapSpace, leafDictionary, this.Mode);
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

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.DrawWorldRectangleOnScreen(this.ComputeBounds(), Color.Red, 1f);

        }
    }
}
