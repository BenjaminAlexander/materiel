using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.DataStuctures;
using Microsoft.Xna.Framework;
namespace MyGame.materiel
{
    public class VehiclePosition : GameObject
    {
        private Vector2GameObjectMember position;
        private GameObjectReferenceListField<CombatVehicle> vehicles;
        private GameObjectReferenceField<Company> company;
        private GameObjectReferenceField<CombatVehicle> inPosition;

        public VehiclePosition(GameObjectCollection collection)
            : base(collection)
        {
            this.position = new Vector2GameObjectMember(this, new Vector2(0));
            this.vehicles = new GameObjectReferenceListField<CombatVehicle>(this);
            this.company = new GameObjectReferenceField<Company>(this);
            this.inPosition = new GameObjectReferenceField<CombatVehicle>(this);
        }

        public static VehiclePosition Factory(GameObjectCollection collection, Company company, Vector2 position)
        {
            VehiclePosition rtn = new VehiclePosition(collection);
            rtn.company.Value = company;
            rtn.position.Value = position;
            collection.Add(rtn);
            return rtn;
        }

        public void Add(CombatVehicle vic)
        {
            vehicles.Value.Add(vic);
        }

        public void Remove(CombatVehicle vic)
        {
            vehicles.RemoveAllReferences(vic);
        }

        public CombatVehicle LastVehicle
        {
            get
            {
                CombatVehicle rtn = null;
                foreach (CombatVehicle vic in vehicles.Value)
                {
                    if (vic != null)
                    {
                        if (rtn == null)
                        {
                            rtn = vic;
                        }
                        else
                        {
                            if (rtn.TimeUntilFightingPositionAbondoned() < vic.TimeUntilFightingPositionAbondoned())
                            {
                                rtn = vic;
                            }
                        }
                    }
                }
                return rtn;
            }
        }

        public void OccupyPosition(CombatVehicle vic)
        {
            if (vic != null && vic.VehicleFightingPosition == this && vic.Position == this.Position)
            {
                if (this.inPosition.Value != null && this.inPosition.Value != vic)
                {
                    CombatVehicle otherVic = this.inPosition.Value;
                    if (otherVic.VehicleFightingPosition == this && otherVic.Position == this.Position)
                    {
                        if (otherVic.TimeUntilFightingPositionAbondoned() > vic.TimeUntilFightingPositionAbondoned())
                        {
                            CombatVehicle swap = vic;
                            vic = otherVic;
                            otherVic = swap;
                        }

                        if (this.company.Value != null)
                        {
                            otherVic.TargetFightingPosition = this.company.Value.NextFightingPosition(otherVic);
                        }
                    }
                }
                this.inPosition.Value = vic;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.position.Value;
            }
        }
    }
}
