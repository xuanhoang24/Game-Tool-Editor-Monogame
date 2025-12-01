using Microsoft.Xna.Framework.Graphics;

namespace Editor.Engine.ECS.Components
{
    public class MeshComponent : IComponent
    {
        public Model Model { get; set; }
        public Texture Texture { get; set; }
        public Effect Shader { get; set; }

        public MeshComponent(Model model, Texture texture, Effect shader)
        {
            Model = model;
            Texture = texture;
            Shader = shader;
        }
    }
}
