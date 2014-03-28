﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    public abstract class PhysicalObject : GameObject
    {
        abstract public new class State : GameObject.State
        {
            private GameObjectReferenceListField<MemberPhysicalObject> memberField = new GameObjectReferenceListField<MemberPhysicalObject>(new List<GameObjectReference<MemberPhysicalObject>>());

            protected override void InitializeFields()
            {
                base.InitializeFields();
                AddField(memberField);
            }

            public State(GameObject obj) : base(obj) { }

            public virtual void Add(MemberPhysicalObject obj)
            {
                Game1.AsserIsServer();
                memberField.Value.Add(new GameObjectReference<MemberPhysicalObject>(obj));
            }

            public abstract Vector2 WorldPosition();

            public abstract float WorldDirection();

            public override void Destroy()
            {
                base.Destroy();
                foreach (GameObjectReference<MemberPhysicalObject> mem in memberField.Value)
                {
                    if (mem.CanDereference())
                    {
                        mem.Dereference().Destroy();
                    }
                }
            }
        }

        public PhysicalObject(GameObjectUpdate message) : base(message) { }
        public PhysicalObject() : base() { }


        public abstract CompositePhysicalObject Root();


        public void Add(MemberPhysicalObject obj)
        {
            Game1.AsserIsServer();
            this.PracticalState<PhysicalObject.State>().Add(obj);
        }
    }
}
