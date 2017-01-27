﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.Geometry;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class InternalNode : Node
    {
        private List<Node> children = new List<Node>();
        private int unitCount = 0;
        private GameObjectField.Modes mode;

        public override int ObjectCount()
        {
            if(unitCount != this.CompleteList().Count)
            {
                throw new Exception("Count mismatch");
            }
            return unitCount;
        }

        public InternalNode(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary, GameObjectField.Modes mode)
            : base(parent, mapSpace, leafDictionary, mode)
        {
            this.mode = mode;

            int halfWidth = mapSpace.Width / 2;
            int halfHeight = mapSpace.Height / 2;

            this.AddLeaf(mapSpace.X, mapSpace.Y, halfWidth, halfHeight);
            this.AddLeaf(mapSpace.X + halfWidth, mapSpace.Y, mapSpace.Width - halfWidth, halfHeight);
            this.AddLeaf(mapSpace.X, mapSpace.Y + halfHeight, halfWidth, mapSpace.Height - halfHeight);
            this.AddLeaf(mapSpace.X + halfWidth, mapSpace.Y + halfHeight, mapSpace.Width - halfWidth, mapSpace.Height - halfHeight);
        }

        private void AddLeaf(int x, int y, int width, int height)
        {
            Rectangle rectangle = new Rectangle(x, y, width, height);
            Leaf leaf = new Leaf(this, rectangle, leafDictionary, this.mode);
            children.Add(leaf);

            if (children.Count > 4)
            {
                throw new Exception("To many children");
            }
        }

        public override bool Add(PhysicalObject obj)
        {
            if (this.Contains(this.GetObjPosition(obj)))
            {
                foreach (Node child in new List<Node>(children))
                {
                    if (child.Add(obj))
                    {
                        unitCount++;
                        return true;
                    }
                }
                throw new Exception("failed adds to QuadTree");
            }
            if (Parent == null)
            {
                //throw new Exception("move out of bounds");
            }

            return false;
        }

        public override Leaf Remove(PhysicalObject obj)
        {
            if (this.Contains(this.GetObjPosition(obj)))
            {
                if (children.Count != 4)
                {
                    throw new Exception("child");
                }

                Leaf removedFrom = null;
                foreach (Node child in children)
                {
                    removedFrom = child.Remove(obj);
                    if (removedFrom != null)
                    {
                        break;
                    }
                }

                if (removedFrom != null)
                {
                    unitCount--;
                    return removedFrom;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        
        public void Collapse()
        {
            if (this.ObjectCount() < Node.max_count)
            {
                if (AllChildrenAreLeaves())
                {
                    if (this.Parent != null)
                    {
                        if (!this.Parent.IsChild(this))
                        {
                            throw new Exception("incorrect child/parent");
                        }
                        Leaf newNode = new Leaf(this.Parent, this.MapSpace, leafDictionary, this.Mode);
                        this.Parent.Replace(this, newNode);

                        foreach (Leaf leaf in children)
                        {
                            foreach (PhysicalObject myObjects in leaf.CompleteList())
                            {
                                this.Remove(myObjects);
                                if (!newNode.Add(myObjects))
                                {
                                    this.Parent.Move(myObjects);
                                }
                            }

                            leafDictionary.DestroyLeaf(leaf);
                        }
                        this.Parent.Collapse();
                    }
                }
                else
                {
                    throw new Exception("Children did not collapse");
                }
            }
        }

        public override List<T> GetObjectsInCircle<T>(Vector2 center, float radius, List<T> list)
        {
            if ((new Circle(center, radius)).Contains(this.MapSpace))
            {
                //return everything
                return this.CompleteList(ref list);
            }
            else
            {
                //List<CompositePhysicalObject> returnList = new List<CompositePhysicalObject>();
                if (this.ObjectCount() > 0)
                {
                    Vector2 rectangleCenter = new Vector2((((float)MapSpace.X) + ((float)MapSpace.Width) / 2), (((float)MapSpace.Y) + ((float)MapSpace.Height) / 2));
                    float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(MapSpace.X, MapSpace.Y));


                    if (Vector2.Distance(rectangleCenter, center) <= radius + rectangleRadius)
                    {
                        foreach (Node child in children)
                        {
                            child.GetObjectsInCircle(center, radius, list);
                        }
                    }
                }
                return list;
            }
        }

        public override List<T> GetObjects<T>(Vector2 point, List<T> list)
        {
            if(this.Bounds.Contains(point) && this.ObjectCount() > 0)
            {
                foreach (Node child in children)
                {
                    child.GetObjects(point, list);
                }
            }
            return list;
        }

        public override List<T> CompleteList<T>(ref List<T> list)
        {
            foreach (Node child in children)
            {
                list = child.CompleteList<T>(ref list);
            }
            return list;
        }

        public override void Move(PhysicalObject obj)
        {
            unitCount--;
            if (this.Contains(this.GetObjPosition(obj)))
            {
                this.Add(obj);
            }
            else
            {
                if (this.Parent != null)
                {
                    this.Parent.Move(obj);
                }
                else
                {
                    throw new Exception("move out of bounds");
                }
            }
        }

        public void Replace(Node current, Node newNode)
        {
            if (current is InternalNode == newNode is InternalNode)
            {
                throw new Exception("Illegal replacement type");
            }

            if (!children.Contains(current))
            {
                throw new Exception("Cannot replace a non child");
            }

            children.Remove(current);
            children.Add(newNode);

            if (children.Count != 4)
            {
                throw new Exception("incorrect child count");
            }
        }

        private bool AllChildrenAreLeaves()
        {
            foreach (Node child in children)
            {
                if (!(child is Leaf))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsChild(Node node)
        {
            return this.children.Contains(node);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            graphics.DrawWorldRectangleOnScreen(this.ComputeBounds(), Color.Red, 1f);
            foreach (Node n in children)
            {
                n.Draw(gameTime, graphics);
            }
        }

        public Rectangle ComputeBounds()
        {
            if (children.Count > 0)
            {
                Rectangle bounds = children[0].Bounds;
                foreach (Node obj in children)
                {
                    bounds = Utils.MathUtils.RectangleUnion(bounds, obj.Bounds);
                }
                this.Bounds = bounds;
            }
            else
            {
                this.Bounds = new Rectangle(this.MapSpace.X, this.MapSpace.Y, 0, 0);
            }

            if (this.Parent != null)
            {
                this.Parent.ComputeBounds();
            }
            return this.Bounds;
        }

        public override T GetClosest<T>(Vector2 point, Select<T> selectFunc, T best)
        {
            float bestDistance;
            if (best == null)
            {
                bestDistance = float.PositiveInfinity;
            }
            else
            {
                bestDistance = Vector2.Distance(point, best.Position);
            }

            foreach(Node node in children)
            {
                if(node.MinimumPositionDistance(point) < bestDistance)
                {
                    T newBest = node.GetClosest<T>(point, selectFunc, best);
                    if(newBest != null)
                    {
                        float newBestDistance = Vector2.Distance(point, newBest.Position);
                        if(newBestDistance < bestDistance)
                        {
                            best = newBest;
                            bestDistance = newBestDistance;
                        }
                    }
                }
            }

            return best;
        }
    }
}