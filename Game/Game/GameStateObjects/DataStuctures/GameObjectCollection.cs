using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Server;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectCollection
    {
        private int nextId = 1;
        private GameObjectListManager listManager = new GameObjectListManager();
        private QuadTree simulationTree;
        private QuadTree previousTree;
        private QuadTree drawTree;
        private Dictionary<int, GameObject> dictionary = new Dictionary<int, GameObject>();
        private Utils.RectangleF worldRectangle;
        
        public int NextID
        {
            get { return nextId++; }
        }

        public QuadTree Tree
        {
            get 
            {
                if (GameObjectField.IsModeSimulation())
                {
                    return simulationTree;
                }
                else if (GameObjectField.IsModePrevious())
                {
                    return previousTree;
                }
                else
                {
                    return drawTree;
                }
            }
        }

        public void MoveInTree(PhysicalObject obj)
        {
            this.simulationTree.Move(obj);
            this.previousTree.Move(obj);
            this.drawTree.Move(obj);
        }

        public RectangleF GetWorldRectangle()
        {
            return worldRectangle;
        }

        public GameObjectCollection(Vector2 world)
        {
            worldRectangle = new Utils.RectangleF(new Vector2(0), world);
            simulationTree = new QuadTree(world, new GetTreePosition(PhysicalObject.GetSimulationPosition));
            previousTree = new QuadTree(world, new GetTreePosition(PhysicalObject.GetPreviousPosition));
            drawTree = new QuadTree(world, new GetTreePosition(PhysicalObject.GetDrawPosition));
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
                if (obj is PhysicalObject)
                {
                    if (simulationTree.Add((PhysicalObject)obj))
                    {
                        previousTree.Add((PhysicalObject)obj);
                        drawTree.Add((PhysicalObject)obj);
                        dictionary.Add(obj.ID, obj);
                        listManager.Add(obj);
                    }
                }
                else
                {
                    dictionary.Add(obj.ID, obj);
                    listManager.Add(obj);
                }
            }
        }

        private void Remove(GameObject obj)
        {
            listManager.Remove(obj);
            dictionary.Remove(obj.ID);
            if (obj is PhysicalObject)
            {
                simulationTree.Remove((PhysicalObject)obj);
                previousTree.Remove((PhysicalObject)obj);
                drawTree.Remove((PhysicalObject)obj);
            }
        }

        public T Get<T>(int id) where T : GameObject
        {
            if (id == 0)
            {
                return null;
            }
            return (T)dictionary[id];
        }

        public GameObjectListManager GetMasterList()
        {
            return listManager;
        }

        public void ServerUpdate(Lobby lobby, GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.ServerUpdate(secondsElapsed);
            }

            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.SendUpdateMessage(lobby, gameTime);
                if (obj.IsDestroyed)
                {
                    this.Remove(obj);
                }
            }
        }

        public void ClientUpdate(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                obj.ClientUpdate(secondsElapsed);
            }

            foreach (GameObject obj in this.listManager.GetList<GameObject>())
            {
                if (obj.IsDestroyed)
                {
                    this.Remove(obj);
                }
            }
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics, Camera camera)
        {
            graphics.BeginWorld();
            foreach (GameObject obj in listManager.GetList<GameObject>())
            {
                obj.Draw(gameTime, graphics);
            }
            graphics.EndWorld();

            graphics.Begin();            
            graphics.DrawRectangle(camera.WorldToScreenPosition(new Vector2(0)),
                camera.WorldToScreenPosition(this.worldRectangle.Size) - camera.WorldToScreenPosition(new Vector2(0))
                , new Vector2(0), 0, Color.Black, 1);

            graphics.End();
        }
    }
}
