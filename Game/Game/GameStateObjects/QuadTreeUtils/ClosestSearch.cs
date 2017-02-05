using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class ClosestSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private T best = null;
        private float bestDistance = float.PositiveInfinity;
        private Vector2 point;
        private Select<T> selectFunc;

        public T Closest
        {
            get
            {
                return best;
            }
        }

        public ClosestSearch(GameObjectField.Modes mode, Vector2 point, Select<T> selectFunc, T best) : this(mode, point, selectFunc)
        {
            this.best = best;
            if (this.best != null)
            {
                this.bestDistance = Vector2.Distance(point, this.best.GetPosition(this.Mode));
            }
            else
            {
                this.bestDistance = float.PositiveInfinity;
            }
        }

        public ClosestSearch(GameObjectField.Modes mode, Vector2 point, Select<T> selectFunc) : base(mode)
        {
            this.selectFunc = selectFunc;
            this.point = point;
        }

        public override void ExamineObject(T obj)
        {
            float newBestDistance = Vector2.Distance(point, obj.GetPosition(this.Mode));
            if (selectFunc(obj) && newBestDistance < bestDistance)
            {
                best = obj;
                bestDistance = newBestDistance;
            }
        }

        public override bool SelectNode(Node node)
        {
            return node.MinimumPositionDistance(point) < bestDistance;
        }

        public override bool SelectParentNode(Node currentNode)
        {
            //return false if the minimum distance out of the current node is more then the best distance
            return Utils.Vector2Utils.DistanceInside(currentNode.MapSpace, this.point) <= this.bestDistance;
        }
    }
}
