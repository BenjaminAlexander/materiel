using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.GameStateObjects;

namespace MyGame.materiel
{
    public interface IPlayerControlled
    {
        GameObjectReference<PlayerGameObject> ControllingPlayer
        {
            get;
        }
    }
}
