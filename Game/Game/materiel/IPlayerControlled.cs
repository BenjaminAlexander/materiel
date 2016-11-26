using MyGame.GameStateObjects;
using MyGame.materiel.GameObjects;

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
