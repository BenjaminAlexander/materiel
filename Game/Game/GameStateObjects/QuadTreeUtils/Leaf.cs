﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class Leaf : Node
    {
        private GameObjectListManager unitList;

        public void CollapseBuild(Leaf other, InternalNode oldInternal)
        {
            foreach (PhysicalObject myObjects in other.CompleteList())
            {
                leafDictionary.SetLeaf(myObjects, null);
                other.unitList.Remove(myObjects);
                if (this.Contains(this.GetObjPosition(myObjects)))
                {
                    unitList.Add(myObjects);
                    leafDictionary.SetLeaf(myObjects, this);
                }
                else
                {
                    throw new Exception("object/node mismatch");
                }

                if (ObjectCount() > max_count)
                {
                    throw new Exception("you should never need to expand while collapsing");
                }
            }

            leafDictionary.DestroyLeaf(other);
        }

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

        public Leaf(InternalNode parent, int x, int y, int width, int height, LeafDictionary leafDictionary, GameObjectField.Modes mode)
            : this(parent, new Rectangle(x, y, width, height), leafDictionary, mode)
        {

        }

        public Rectangle ComputeBounds()
        {
            List<PhysicalObject> units = unitList.GetList<PhysicalObject>();
            if (units.Count > 0)
            {
                Rectangle bounds = units[0].BoundingRectangle;
                foreach (PhysicalObject obj in units)
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
                this.Parent.IncrementCount();

                if (ObjectCount() > max_count)
                {
                    this.Expand();
                }
                this.ComputeBounds();
                return true;
            }
            return false;
        }

        public override bool Remove(PhysicalObject unit)
        {
            if (unitList.Contains(unit))
            {
                leafDictionary.SetLeaf(unit, null);
                unitList.Remove(unit);
                this.Parent.DecrementCount();
                this.ComputeBounds();
                this.Collapse();
                return true;
            }
            return false;
        }

        public void Collapse()
        {
            this.Parent.Collapse();
        }

        public override void Move(PhysicalObject obj)
        {
            if (unitList.Contains(obj))
            {
                if (!this.Contains(this.GetObjPosition(obj)))
                {
                    
                    leafDictionary.SetLeaf(obj, null);
                    unitList.Remove(obj);
                    this.Parent.DecrementCount();
                    this.ComputeBounds();
                    
                    this.Parent.Move(obj);
                    if (unitList.Contains(obj))
                    {
                        throw new Exception("Move failed");
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
                Node newNode = new InternalNode(null, this.MapSpace, leafDictionary, this.Mode);
                
                foreach (PhysicalObject obj in unitList.GetList<PhysicalObject>())
                {
                    leafDictionary.SetLeaf(obj, null);
                    unitList.Remove(obj);
                    if (!newNode.Add(obj))
                    {
                        throw new Exception("Failed to add after move");
                    }
                }
                this.Parent.Replace(this, newNode);
                leafDictionary.DestroyLeaf(this);
            }
        }

        public Boolean Contains(PhysicalObject obj)
        {
            return this.unitList.Contains(obj);
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

        public override List<T> GetObjects<T>(Vector2 point, List<T> list)
        {
            if (this.Bounds.Contains(point) && ObjectCount() > 0)
            {
                foreach (PhysicalObject unit in unitList.GetList<PhysicalObject>())
                {
                    if (unit is T && unit.CollidesWith(point))
                    {
                        list.Add((T)unit);
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

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.DrawWorldRectangleOnScreen(this.ComputeBounds(), Color.Red, 1f);

        }

        public override T GetClosest<T>(Vector2 point, Select<T> selectFunc, T best)
        {
            List<T> objList = new List<T>();
            objList = this.CompleteList<T>(ref objList);

            float bestDistance;
            if (best == null)
            {
                bestDistance = float.PositiveInfinity;
            }
            else
            {
                bestDistance = Vector2.Distance(point, best.Position);
            }

            foreach (T obj in objList)
            {
                float newBestDistance = Vector2.Distance(point, obj.Position);
                if (selectFunc(obj))
                {
                    if (newBestDistance < bestDistance)
                    {
                        best = obj;
                        bestDistance = newBestDistance;
                    }
                }
            }

            return best;
        }
    }
}
