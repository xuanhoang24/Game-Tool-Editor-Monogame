using Editor.Engine.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Editor.Engine.ECS.Systems
{
    public class RenderSystem : ISystem
    {
        public void Update(World world, GameTime gameTime)
        {
            var camera = world.GetCamera();
            if (camera == null) return;

            foreach (var entity in world.GetEntities())
            {
                if (entity.HasComponent<MeshComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var mesh = entity.GetComponent<MeshComponent>();
                    var transform = entity.GetComponent<TransformComponent>();

                    Matrix worldMatrix = transform.GetWorldMatrix();
                    mesh.Shader.Parameters["World"].SetValue(worldMatrix);
                    mesh.Shader.Parameters["WorldViewProjection"].SetValue(worldMatrix * camera.View * camera.Projection);
                    mesh.Shader.Parameters["Texture"].SetValue(mesh.Texture);

                    foreach (ModelMesh modelMesh in mesh.Model.Meshes)
                    {
                        modelMesh.Draw();
                    }
                }
            }
        }
    }
}
