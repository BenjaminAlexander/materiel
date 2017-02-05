using System;
using MyGame.Geometry;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class CircleSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private Circle circle;
        private List<T> rtn = new List<T>();

        public List<T> List
        {
            get
            {
                return rtn;
            }
        }

        public CircleSearch(GameObjectField.Modes mode, Vector2 point, float radius) : this(mode, new Circle(point, radius))
        {
        }

        public CircleSearch(GameObjectField.Modes mode, Circle circle) : base(mode)
        {
            this.circle = circle;
        }

        public override void ExamineObject(T obj)
        {
            if (Vector2.Distance(obj.GetPosition(this.Mode), circle.Center) <= circle.Radius)
            {
                rtn.Add(obj);
            }
        }

        public override bool SelectNode(Node node)
        {
            Vector2 rectangleCenter = new Vector2((((float)node.MapSpace.X) + ((float)node.MapSpace.Width) / 2), (((float)node.MapSpace.Y) + ((float)node.MapSpace.Height) / 2));
            float rectangleRadius = Vector2.Distance(rectangleCenter, new Vector2(node.MapSpace.X, node.MapSpace.Y));

            return Vector2.Distance(rectangleCenter, this.circle.Center) <= this.circle.Radius + rectangleRadius;
        }

        public override bool SelectParentNode(Node currentNode)
        {
            //true if currentNode does not completely contain the circle
            Rectangle r = currentNode.MapSpace;

            float x = Math.Min(circle.Center.X - r.X, r.X + r.Width - circle.Center.X);
            float y = Math.Min(circle.Center.Y - r.Y, r.Y + r.Height - circle.Center.Y);

            return x < circle.Radius || y < circle.Radius;
        }
    }
}
