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
            collection.Add(rtn);
            rtn.company.Value = company;
            rtn.position.Value = position;
            return rtn;
        }

        public void Add(CombatVehicle vic)
        {
            if (vic.VehicleFightingPosition != null)
            {
                vic.VehicleFightingPosition.Remove(vic);
            }
            vehicles.Value.Add(vic);
            vic.VehicleFightingPosition = this;
        }

        public void Remove(CombatVehicle vic)
        {
            vehicles.RemoveAllReferences(vic);
            vic.VehicleFightingPosition = null;
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
                            this.company.Value.AssignFightingPosition(otherVic);
                        }
                    }
                }
                this.inPosition.Value = vic;
            }
        }

        public float TimeUntilAbandoned()
        {
            CombatVehicle vic = this.LastVehicle;
            if (vic == null)
            {
                return 0;
            }
            else
            {
                return vic.TimeUntilFightingPositionAbondoned();
            }
        }

        public float TimeUntilAbandoned(CombatVehicle vic)
        {
            if (vic == null)
            {
                return 0;
            }
            else
            {
                return vic.TimeUntilFightingPositionAbondoned(this.Position);
            }
        }

        public override void Destroy()
        {
            foreach (CombatVehicle vic in this.vehicles.Value.ToArray())
            {
                this.Remove(vic);
            }
            base.Destroy();
        }

        public void DrawScreen(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, Camera camera, Color color, float depth)
        {
            Vector2 screenPos = camera.WorldToScreenPosition(this.Position);
            graphics.DrawCircle(screenPos, 15, color, depth);
        }

        public Vector2 Position
        {
            get
            {
                return this.position.Value;
            }
        }

        public void Add(VehiclePosition pos)
        {
            foreach (CombatVehicle vic in pos.vehicles.Value.ToArray())
            {
                this.Add(vic);
            }
        }
    }
}
