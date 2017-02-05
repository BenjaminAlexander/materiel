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

        public Leaf(InternalNode oldNode)
            : this(oldNode.Parent, oldNode.MapSpace, oldNode.LeafDictionary, oldNode.Mode)
        {
            foreach (PhysicalObject myObjects in oldNode.CompleteList())
            {
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

            oldNode.Parent.Replace(oldNode, this);
            this.ComputeBounds();
        }

        public override int ObjectCount()
        {
            return unitList.GetMaster().Count;
        }

        public Leaf(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary, GameObjectField.Modes mode)
            : base(parent, mapSpace, leafDictionary, mode)
        {
            unitList = new GameObjectListManager();
            this.Bounds = null;
        }

        public Leaf(InternalNode parent, int x, int y, int width, int height, LeafDictionary leafDictionary, GameObjectField.Modes mode)
            : this(parent, new Rectangle(x, y, width, height), leafDictionary, mode)
        {

        }

        public Rectangle? ComputeBounds()
        {
            List<PhysicalObject> units = unitList.GetList<PhysicalObject>();
            if (units.Count > 0)
            {
                Rectangle bounds = units[0].GetFastBoundingRectangle(this.Mode);
                foreach (PhysicalObject obj in units)
                {
                    bounds = Utils.MathUtils.RectangleUnion(bounds, obj.GetFastBoundingRectangle(this.Mode));
                }
                this.Bounds = bounds;
            }
            else
            {
                this.Bounds = null;
            }
            this.Parent.ComputeBounds();
            return this.Bounds;
        }

        public override bool Add(PhysicalObject unit)
        {
            if (this.Contains(this.GetObjPosition(unit)))
            {
                if (ObjectCount() >= max_count && MapSpace.Width > 1 && MapSpace.Height > 1)
                {
                    InternalNode newNode = new InternalNode(this);
                    return newNode.Add(unit);
                }
                else
                {
                    unitList.Add(unit);
                    leafDictionary.SetLeaf(unit, this);
                    this.ComputeBounds();
                    return true;
                }
            }
            return false;
        }

        public override bool Remove(PhysicalObject unit)
        {
            if (unitList.Contains(unit))
            {
                leafDictionary.SetLeaf(unit, null);
                unitList.Remove(unit);
                this.ComputeBounds();
                return true;
            }
            return false;
        }

        public void Move(PhysicalObject obj)
        {
            if (unitList.Contains(obj))
            {
                if (!this.Contains(this.GetObjPosition(obj)))
                {
                    leafDictionary.SetLeaf(obj, null);
                    unitList.Remove(obj);
                    
                    this.Parent.Move(obj);
                    if (unitList.Contains(obj))
                    {
                        throw new Exception("Move failed");
                    }
                }
                this.ComputeBounds();
            }
            else
            {
                throw new Exception("No such object");
            }
        }

        public Boolean Contains(PhysicalObject obj)
        {
            return this.unitList.Contains(obj);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            if (this.Bounds != null)
            {
                graphics.DrawWorldRectangleOnScreen((Rectangle)this.Bounds, Color.Blue, 1f);
            }
            foreach(PhysicalObject obj in unitList.GetList<PhysicalObject>())
            {
                graphics.DrawWorldRectangleOnScreen(obj.GetFastBoundingRectangle(this.Mode), Color.Blue, 1f);
            }
        }

        public override void SearchNode<T>(QuadTreeSearch<T> searchObj)
        {
            foreach(T obj in unitList.GetList<T>())
            {
                searchObj.ExamineObject(obj);
            }
        }
    }
}
