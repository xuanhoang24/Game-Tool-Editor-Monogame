using Microsoft.Xna.Framework;

namespace Editor.Engine.ECS.Systems
{
    public interface ISystem
    {
        void Update(World world, GameTime gameTime);
    }
}
