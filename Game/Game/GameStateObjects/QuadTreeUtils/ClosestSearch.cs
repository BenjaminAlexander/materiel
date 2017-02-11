using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.GameStateObjects.QuadTreeUtils
{
    class ClosestSearch<T> : QuadTreeSearch<T> where T : PhysicalObject
    {
        private T best = null;
        private float bestDistance = float.PositiveInfinity;
        private Vector2 point;
        private Select<T> selectFunc;

        public static T GetObject(GameObjectCollection collection, Vector2 point, Select<T> selectFunc)
        {
            ClosestSearch<T> searchObj = new ClosestSearch<T>(point, selectFunc);
            collection.SearchDownFromRoot<T>(searchObj);
            return searchObj.Closest;
        }

        public static T GetObject(GameObjectCollection collection, Vector2 point, Select<T> selectFunc, T best)
        {
            ClosestSearch<T> searchObj = new ClosestSearch<T>(GameObjectField.Mode, point, selectFunc, best);
            collection.SearchDownFromRoot<T>(searchObj);
            return searchObj.Closest;
        }

        public static T GetObject(GameObjectCollection collection, PhysicalObject obj, Select<T> selectFunc, T best)
        {
            ClosestSearch<T> searchObj = new ClosestSearch<T>(GameObjectField.Mode, obj.GetPosition(GameObjectField.Mode), selectFunc, best);
            collection.SearchUpFromLeaf<T>(searchObj, obj);
            return searchObj.Closest;
        }

        public T Closest
        {
            get
            {
                return best;
            }
        }

        public ClosestSearch(GameObjectField.Modes mode, Vector2 point, Select<T> selectFunc, T best) : this(point, selectFunc)
        {
            this.best = best;
            if (this.best != null)
            {
                this.bestDistance = Vector2.Distance(point, this.best.GetPosition(mode));
            }
            else
            {
                this.bestDistance = float.PositiveInfinity;
            }
        }

        public ClosestSearch(Vector2 point, Select<T> selectFunc)
        {
            this.selectFunc = selectFunc;
            this.point = point;
        }

        public override void ExamineObject(T obj, GameObjectField.Modes mode)
        {
            float newBestDistance = Vector2.Distance(point, obj.GetPosition(mode));
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
