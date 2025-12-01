using Microsoft.Xna.Framework;

namespace Editor.Engine.ECS.Components
{
    public class TransformComponent : IComponent
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Scale { get; set; }

        public TransformComponent(Vector3 position, float scale = 1.0f)
        {
            Position = position;
            Rotation = Vector3.Zero;
            Scale = scale;
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.CreateScale(Scale) *
                   Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                   Matrix.CreateTranslation(Position);
        }
    }
}
