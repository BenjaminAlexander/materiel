﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.PlayerControllers;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects.Ships
{
    class PlayerRotatingGun : Turret, GunnerObserver
    {
        Boolean pointedAtTarget = false;
        GunnerController controller;
        public PlayerRotatingGun(int id)
            : base(id)
        {
            

        }

        public PlayerRotatingGun(Ship parent, Vector2 positionRelativeToParent, float directionRelativeToParent, GunnerController controller)
            : base(parent, positionRelativeToParent, directionRelativeToParent, (float)(Math.PI * .5))
        {
            
            controller.Register(this);
            this.controller = controller;

            Gun gun1 = new Gun(this, new Vector2(50f, 10), 0);
            GameObject.Collection.Add(gun1);
            Gun gun2 = new Gun(this, new Vector2(50f, -10), 0);
            GameObject.Collection.Add(gun2);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (Game1.IsServer)
            {
                base.UpdateSubclass(gameTime);
                Vector2 target = controller.AimPointInWorld;
                this.Target = target;
                pointedAtTarget = IsPointedAt(target, 50);
            }
        }

        public void UpdateWithEvent(GunnerEvent e)
        {
            if (e is GunnerFire && pointedAtTarget)
            {
                Fire();
            }
        }
    }
}
