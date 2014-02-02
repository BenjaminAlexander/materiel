﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectCollection
    {
        private GameObjectListManager listManager = new GameObjectListManager();
        //private GameObjectListManager updateList = new GameObjectListManager();
        private QuadTree quadTree;
        private Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();


        public GameObjectCollection(Vector2 world)
        {
            quadTree = new QuadTree(world);
        }

        public Boolean Contains(GameObject obj)
        {
            return dictionary.ContainsKey(obj.ID);
        }

        public Boolean Contains(int id)
        {
            return dictionary.ContainsKey(id);
        }

        public void Add(GameObject obj)
        {
            if (!this.Contains(obj))
            {
                dictionary.Add(obj.ID, obj);
                listManager.Add(obj);
                if (obj is CompositePhysicalObject)
                {
                    quadTree.Add((CompositePhysicalObject)obj);
                }

                if (Game1.IsServer)
                {
                    Game1.outgoingQue.Enqueue(obj.GetUpdateMessage());
                }
            }
        }


        private void Remove(GameObject obj)
        {
            listManager.Remove(obj);
            dictionary.Remove(obj.ID);
            if (obj is CompositePhysicalObject)
            {
                quadTree.Remove((CompositePhysicalObject)obj);
            }
        }

        public void ApplyMessage(TCPMessage m)
        {
            if (!Game1.IsServer && m is GameObjectCollectionUpdate)
            {
                GameObjectCollectionUpdate updateMessage = (GameObjectCollectionUpdate)m;
                updateMessage.Apply(this);
            }
        }

        public List<CompositePhysicalObject> GetObjectsInCircle(Vector2 position, float radius)
        {
            return quadTree.GetObjectsInCircle(position, radius);
        }

        public void CleanUp()
        {
            List<GameObject> objList = new List<GameObject>(listManager.GetList<GameObject>());

            foreach (GameObject obj in objList)
            {
                obj.SendUpdateMessage();
            }

            foreach (GameObject obj in objList)
            {
                if (obj.IsDestroyed)
                {
                    this.Remove(obj);
                }
            }
        }

        public GameObject Get(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return dictionary[id];
        }

        public GameObjectListManager GetMasterList()
        {
            return listManager;
        }
    }
}
