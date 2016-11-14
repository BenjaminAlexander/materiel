﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework;
using MyGame.Server;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.materiel
{
    public class CombatVehicle : Vehicle
    {
        public const float maxMateriel = 5;
        private Vector2GameObjectMember targetPosition;

        public CombatVehicle(GameObjectCollection collection)
            : base(collection)
        {
            targetPosition = new Vector2GameObjectMember(this, new Vector2(0));
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
    }
}