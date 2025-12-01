using Editor.Engine.ECS.Components;
using Microsoft.Xna.Framework;
using System;

namespace Editor.Engine.ECS.Systems
{
    public class OrbitSystem : ISystem
    {
        public void Update(World world, GameTime gameTime)
        {
            foreach (var entity in world.GetEntities())
            {
                if (entity.HasComponent<OrbitComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var orbit = entity.GetComponent<OrbitComponent>();
                    var transform = entity.GetComponent<TransformComponent>();
                    
                    var parent = world.GetEntity(orbit.ParentEntityId);
                    if (parent != null && parent.HasComponent<TransformComponent>())
                    {
                        var parentTransform = parent.GetComponent<TransformComponent>();
                        orbit.OrbitAngle += orbit.OrbitSpeed;

                        transform.Position = new Vector3(
                            (float)(Math.Cos(orbit.OrbitAngle) * orbit.OrbitRadius) + parentTransform.Position.X,
                            transform.Position.Y,
                            (float)(Math.Sin(orbit.OrbitAngle) * orbit.OrbitRadius) + parentTransform.Position.Z
                        );
                    }
                }
            }
        }
    }
}
