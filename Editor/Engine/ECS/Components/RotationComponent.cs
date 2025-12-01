using Microsoft.Xna.Framework;

namespace Editor.Engine.ECS.Components
{
    public class RotationComponent : IComponent
    {
        public Vector3 RotationSpeed { get; set; }

        public RotationComponent(float x, float y, float z)
        {
            RotationSpeed = new Vector3(x, y, z);
        }
    }
}
