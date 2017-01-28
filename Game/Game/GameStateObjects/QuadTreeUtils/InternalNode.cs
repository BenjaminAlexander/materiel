using System;
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

        public void IncrementCount()
        {
            unitCount++;
            if(this.Parent != null)
            {
                this.Parent.IncrementCount();
            }
        }

        public void DecrementCount()
        {
            unitCount--;
            if (this.Parent != null)
            {
                this.Parent.DecrementCount();
            }
        }

        public InternalNode(InternalNode parent, Rectangle mapSpace, LeafDictionary leafDictionary, GameObjectField.Modes mode)
            : base(parent, mapSpace, leafDictionary, mode)
        {
            this.mode = mode;

            int halfWidth = mapSpace.Width / 2;
            int halfHeight = mapSpace.Height / 2;

            children.Add(new Leaf(this, mapSpace.X, mapSpace.Y, halfWidth, halfHeight, leafDictionary, this.mode));
            children.Add(new Leaf(this, mapSpace.X + halfWidth, mapSpace.Y, mapSpace.Width - halfWidth, halfHeight, leafDictionary, this.mode));
            children.Add(new Leaf(this, mapSpace.X, mapSpace.Y + halfHeight, halfWidth, mapSpace.Height - halfHeight, leafDictionary, this.mode));
            children.Add(new Leaf(this, mapSpace.X + halfWidth, mapSpace.Y + halfHeight, mapSpace.Width - halfWidth, mapSpace.Height - halfHeight, leafDictionary, this.mode));
        }

        public override bool Add(PhysicalObject obj)
        {
            if (this.Contains(this.GetObjPosition(obj)))
            {
                foreach (Node child in new List<Node>(children))
                {
                    if (child.Add(obj))
                    {
                        return true;
                    }
                }
                throw new Exception("failed adds to QuadTree");
            }
            return false;
        }

        public override bool Remove(PhysicalObject obj)
        {
            if (this.Contains(this.GetObjPosition(obj)))
            {
                if (children.Count != 4)
                {
                    throw new Exception("child");
                }

                foreach (Node child in children)
                {
                    if (child.Remove(obj))
                    {
                        return true;
                    }
                }
            }
            return false;
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
                        Leaf newNode = new Leaf(null, this.MapSpace, leafDictionary, this.Mode);
                        

                        foreach (Leaf leaf in children)
                        {
                            newNode.CollapseBuild(leaf, this);
                        }
                        this.Parent.Replace(this, newNode);
                        newNode.ComputeBounds();
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

            newNode.Parent = this;
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