﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class ObjectIntersectionSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private PhysicalObject obj;
        private Rectangle bound;
        private List<T> rtn = new List<T>();

        public ObjectIntersectionSearch(GameObjectField.Modes mode, PhysicalObject obj) : base(mode)
        {
            this.obj = obj;
            this.bound = obj.GetBoundingRectangle(mode);
        }

        public override void ExamineObject(T other)
        {
            if (other != this.obj && other.CollidesWith(this.obj, this.Mode))
            {
                rtn.Add(other);
            }
        }

        public override bool SelectNode(Node node)
        {
            if (node.Bounds != null)
            {
                Rectangle nodeBound = (Rectangle)node.Bounds;
                return Utils.Vector2Utils.Intersects(nodeBound, bound);
            }
            return false;
        }

        public override bool SelectParentNode(Node currentNode)
        {
            return true;
        }
    }
}
