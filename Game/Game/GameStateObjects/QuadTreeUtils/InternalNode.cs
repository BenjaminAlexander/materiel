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
            int listCount = this.CompleteList().Count;
            if (unitCount != listCount)
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

            children.Add(new Leaf(this, mapSpace.X, mapSpace.Y, halfWidth, halfHeight, leafDictionary, this.mode));
            children.Add(new Leaf(this, mapSpace.X + halfWidth, mapSpace.Y, mapSpace.Width - halfWidth, halfHeight, leafDictionary, this.mode));
            children.Add(new Leaf(this, mapSpace.X, mapSpace.Y + halfHeight, halfWidth, mapSpace.Height - halfHeight, leafDictionary, this.mode));
            children.Add(new Leaf(this, mapSpace.X + halfWidth, mapSpace.Y + halfHeight, mapSpace.Width - halfWidth, mapSpace.Height - halfHeight, leafDictionary, this.mode));
        }

        public InternalNode(Leaf node) : this(node.Parent, node.MapSpace, node.LeafDictionary, node.Mode)
        {
            foreach (PhysicalObject obj in node.CompleteList())
            {
                if (!this.Add(obj))
                {
                    throw new Exception("Failed to add after move");
                }
            }
            node.Parent.Replace(node, this);
        }

        public override bool Add(PhysicalObject obj)
        {
            if (this.Contains(this.GetObjPosition(obj)))
            {
                unitCount++;
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
                foreach (Node child in children)
                {
                    if (child.Remove(obj))
                    {
                        unitCount--;
                        this.Collapse();
                        return true;
                    }
                }
            }
            return false;
        }
        
        public void Collapse()
        {
            if (this.ObjectCount() <= Node.max_count)
            {
                if (AllChildrenAreLeaves())
                {
                    if (this.Parent != null)
                    {
                        Leaf newNode = new Leaf(this);
                    }
                }
                else
                {
                    throw new Exception("Children did not collapse");
                }
            }
        }

        public void Move(PhysicalObject obj)
        {
            unitCount--;
            if (this.Contains(this.GetObjPosition(obj)))
            {
                this.Add(obj);
            }
            else if (this.Parent != null)
            {
                InternalNode parent = this.Parent;
                this.Collapse();
                parent.Move(obj);
            }
            else
            {
                throw new Exception("move out of bounds");
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
            current.Parent = null;

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

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            if (this.Bounds != null)
            {
                graphics.DrawWorldRectangleOnScreen((Rectangle)this.Bounds, Color.Red, 1f);
            }
            foreach (Node n in children)
            {
                n.Draw(gameTime, graphics);
            }
        }

        public Rectangle? ComputeBounds()
        {
            if (this.unitCount > 0)
            {
                this.Bounds = null;
                foreach (Node obj in children)
                {
                    if (obj.Bounds != null)
                    {
                        Rectangle nodeBound = (Rectangle)obj.Bounds;
                        if (this.Bounds == null)
                        {
                            this.Bounds = nodeBound;
                        }
                        else
                        {
                            this.Bounds = Utils.MathUtils.RectangleUnion((Rectangle)this.Bounds, nodeBound);
                        }
                    }
                }
            }
            else
            {
                this.Bounds = null;
            }
            if(this.Parent != null)
            {
                this.Parent.ComputeBounds();
            }
            return this.Bounds;
        }

        public override void SearchNode<T>(QuadTreeSearch<T> searchObj)
        {
            foreach (Node node in children)
            {
                if(searchObj.SelectNode(node))
                {
                    node.SearchNode<T>(searchObj);
                }
            }
        }
    }
}