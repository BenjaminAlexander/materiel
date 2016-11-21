using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel.GameObjects
{
    public abstract class MaterielContainer : PhysicalObject
    {
        public MaterielContainer(GameObjectCollection collection)
            : base(collection)
        {

        }

        public static void ServerInitialize(MaterielContainer obj, Vector2 position, float maxMateriel)
        {
            PhysicalObject.ServerInitialize(obj, position, 0);
        }
    }
}
