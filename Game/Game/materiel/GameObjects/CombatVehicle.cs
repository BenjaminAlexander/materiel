using System;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;

namespace MyGame.materiel.GameObjects
{
    public class CombatVehicle : Vehicle
    {
        public const float maxMateriel = 5;
        private Vector2GameObjectMember targetPosition;
        private GameObjectReferenceField<Vehicle> targetVehicle;
        private FloatGameObjectMember gunCoolDown;

        //angle of the turret relative to the vehicle body
        private InterpolatedAngleGameObjectMember turretAngle;

        public CombatVehicle(GameObjectCollection collection)
            : base(collection)
        {
            targetPosition = new Vector2GameObjectMember(this, new Vector2(0));
            turretAngle = new InterpolatedAngleGameObjectMember(this, 0);
            targetVehicle = new GameObjectReferenceField<Vehicle>(this);
            gunCoolDown = new FloatGameObjectMember(this, 0);
        }

        public static void ServerInitialize(CombatVehicle vic, PlayerGameObject controllingPlayer, Vector2 position)
        {
            Vehicle.ServerInitialize(vic, controllingPlayer, position, maxMateriel);
            vic.targetPosition.Value = position;
        }

        public static CombatVehicle CombatVehicleFactory(GameObjectCollection collection, PlayerGameObject controllingPlayer, Vector2 position)
        {
            CombatVehicle vehicle = new CombatVehicle(collection);
            collection.Add(vehicle);
            CombatVehicle.ServerInitialize(vehicle, controllingPlayer, position);
            return vehicle;
        }

        public Vector2 TargetPosition
        {
            get
            {
                return this.targetPosition.Value;
            }

            set
            {
                this.targetPosition.Value = value;
            }
        }

        public bool SelectEnemyVehicle(Vehicle vic)
        {
            PlayerGameObject player = this.ControllingPlayer;
            if (player != null)
            {
                return !player.Owns(vic);
            }
            else
            {
                return true;
            }
        }

        public override void ServerOnlyUpdate(float secondsElapsed)
        {
            base.ServerOnlyUpdate(secondsElapsed);
            if (this.Materiel >= .5f && gunCoolDown == 0 && this.targetVehicle.Value != null && Vector2.Distance(this.Position, this.targetVehicle.Value.Position) < 1500)
            {
                this.Materiel = this.Materiel - .5f;
                Vector2 bulletPosition = this.Position; //start with vic position
                bulletPosition = bulletPosition + Utils.Vector2Utils.RotateVector2(new Vector2(-5, 0), this.Direction);// add the position of the turret on the vehicle
                bulletPosition = bulletPosition + Utils.Vector2Utils.RotateVector2(new Vector2(27, 0), this.Direction + this.turretAngle);// add the position of the end of the turret

                Bullet.BulletFactory(this.Collection, bulletPosition, this.Direction + this.turretAngle);
                this.gunCoolDown.Value = 1f;
            }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            this.MoveTowardAndIdle(this.TargetPosition, seconds);

            gunCoolDown.Value = Math.Max(gunCoolDown - seconds, 0);
            if (gunCoolDown.Value == 0 && this.targetVehicle.CanDereference)
            {
                this.targetVehicle.Value = ClosestSearch<Vehicle>.GetObject(this.Collection, this, this.SelectEnemyVehicle, this.targetVehicle.Value);
            }
            if(this.targetVehicle.CanDereference && this.targetVehicle.Value != null)
            {
                Vector2 turretPosition = this.Position + Utils.Vector2Utils.RotateVector2(new Vector2(-5, 0), this.Direction);
                this.turretAngle.Value = Utils.Vector2Utils.Vector2Angle(this.targetVehicle.Value.Position - turretPosition) - this.Direction;
            }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);

            LoadedTexture gun = TextureLoader.GetTexture("Gun");
            gun.Draw(graphics, this.Position + Utils.Vector2Utils.RotateVector2(new Vector2(-5, 0), this.Direction), new Vector2(13, 5), turretAngle + this.Direction, Color.White, .91f);
        }
    }
}
