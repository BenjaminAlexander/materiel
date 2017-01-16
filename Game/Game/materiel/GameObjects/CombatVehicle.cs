using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.DrawingUtils;

namespace MyGame.materiel.GameObjects
{
    public class CombatVehicle : Vehicle
    {
        public const float maxMateriel = 5;
        private Vector2GameObjectMember targetPosition;

        //angle of the turret relative to the vehicle body
        private InterpolatedAngleGameObjectMember turretAngle;

        public CombatVehicle(GameObjectCollection collection)
            : base(collection)
        {
            targetPosition = new Vector2GameObjectMember(this, new Vector2(0));
            turretAngle = new InterpolatedAngleGameObjectMember(this, 0);
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

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            this.MoveTowardAndIdle(this.TargetPosition, seconds);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);

            LoadedTexture gun = TextureLoader.GetTexture("Gun");
            gun.Draw(graphics, this.Position + Utils.Vector2Utils.RotateVector2(new Vector2(-5, 0), this.Direction), new Vector2(13, 5), turretAngle + this.Direction, Color.White, .91f);
        }
    }
}
