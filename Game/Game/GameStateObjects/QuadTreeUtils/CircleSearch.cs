using System;
using MyGame.Geometry;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    public class CircleSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private Circle circle;
        private List<T> rtn = new List<T>();

        public static List<T> GetObjects(GameObjectCollection collection, Vector2 center, float radius)
        {
            CircleSearch<T> searchObj = new CircleSearch<T>(center, radius);
            collection.SearchDownFromRoot<T>(searchObj);
            return searchObj.List;
        }

        public List<T> List
        {
            get
            {
                return rtn;
            }
        }

        public CircleSearch(Vector2 point, float radius) : this(new Circle(point, radius))
        {
        }

        public CircleSearch(Circle circle)
        {
            this.circle = circle;
        }

        public override void ExamineObject(T obj, GameObjectField.Modes mode)
        {
            if (Vector2.Distance(obj.GetPosition(mode), circle.Center) <= circle.Radius)
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
