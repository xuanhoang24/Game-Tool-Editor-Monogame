using Editor.Engine.ECS.Components;
using Microsoft.Xna.Framework;

namespace Editor.Engine.ECS.Systems
{
    public class RotationSystem : ISystem
    {
        public void Update(World world, GameTime gameTime)
        {
            foreach (var entity in world.GetEntities())
            {
                if (entity.HasComponent<RotationComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var rotation = entity.GetComponent<RotationComponent>();
                    var transform = entity.GetComponent<TransformComponent>();
                    transform.Rotation += rotation.RotationSpeed;
                }
            }
        }
    }
}
