using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Editor.Engine.Interfaces;
using System.IO;

namespace Editor.Engine
{
    public class Camera : ISerializable
    {
        public Vector3 Position { get; set; } = new Vector3(0, 0, 0);
        public Matrix View { get; set; } = Matrix.Identity;
        public Matrix Projection { get; set; } = Matrix.Identity;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 1000f;
        public float AspectRatio { get; set; } = 16 / 9;

        public Camera()
        {
        }

        public Camera(Vector3 _position, float _aspectRatio)
        {
            Update(_position, _aspectRatio);
        }

        public void Update(Vector3 _position, float _aspectRatio)
        {
            Position = _position;
            AspectRatio = _aspectRatio;
            View = Matrix.CreateLookAt(Position,
                                        new Vector3(0, 0, 0),
                                        Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),
                                                                _aspectRatio,
                                                                NearPlane,
                                                                FarPlane);
        }

        public void Serialize(BinaryWriter _stream)
        {
            HelpSerialize.Vec3(_stream, Position);
            _stream.Write(NearPlane);
            _stream.Write(FarPlane);
            _stream.Write(AspectRatio);

        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            Position = HelpDeserialize.Vec3(_stream);
            NearPlane = _stream.ReadSingle();
            FarPlane = _stream.ReadSingle();
            AspectRatio = _stream.ReadSingle();
            Update(Position, AspectRatio);
        }
    }
}
