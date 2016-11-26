using System;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel.GameObjects
{
    public abstract class MaterielContainer : PhysicalObject
    {

        private FloatGameObjectMember materiel;
        private FloatGameObjectMember maxMateriel;

        public MaterielContainer(GameObjectCollection collection)
            : base(collection)
        {
            materiel = new FloatGameObjectMember(this, 10);
            maxMateriel = new FloatGameObjectMember(this, 10);
        }

        public static void ServerInitialize(MaterielContainer obj, Vector2 position, float direction, float materiel, float maxMateriel)
        {
            obj.materiel.Value = materiel;
            obj.maxMateriel.Value = maxMateriel;
            PhysicalObject.ServerInitialize(obj, position, direction);
        }

        public float Materiel
        {
            get
            {
                return materiel.Value;
            }

            set
            {
                float newValue = Math.Min(value, this.maxMateriel);
                newValue = Math.Max(newValue, 0);
                materiel.Value = newValue;
            }
        }

        public float MaxMateriel
        {
            get
            {
                return maxMateriel.Value;
            }
        }

        public float MaxMaterielDeposit
        {
            get
            {
                return this.MaxMateriel - this.Materiel;
            }
        }

        public virtual float MaxMaterielWithdrawl
        {
            get
            {
                return this.Materiel;
            }
        }

        public static void MaximumMaterielTransfer(MaterielContainer from, MaterielContainer to)
        {
            float resupply = Math.Min(from.MaxMaterielWithdrawl, to.MaxMaterielDeposit);
            resupply = Math.Min(resupply, from.Materiel);

            from.Materiel = from.Materiel - resupply;
            to.Materiel = to.Materiel + resupply;
        }
    }
}
