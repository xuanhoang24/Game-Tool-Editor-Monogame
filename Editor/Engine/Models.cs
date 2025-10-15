using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.IO;

namespace Editor.Engine
{
    [DefaultProperty(nameof(Position))]
    class Models : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public enum DiffuseType
        {
            Metal,
            Grass,
            HeightMap
        }

        // Members
        private ContentManager m_content;
        private Vector3 m_position;
        private Vector3 m_rotation;
        private float m_scale;
        private DiffuseType m_diffuseTexture = DiffuseType.Metal;

        // Accessors
        [Browsable(false)]
        public Model Mesh { get; set; }

        [Browsable(false)]
        public Texture Texture { get; set; }

        [Browsable(false)]
        public Effect Shader { get; set; }

        // Properties Grid Category
        // Appearance
        [Category("Appearance")]
        [DisplayName("Diffuse Texture")]
        [Description("Diffuse texture of the model.")]
        public DiffuseType DiffuseTexture
        {
            get => m_diffuseTexture;
            set
            {
                if (m_diffuseTexture != value)
                {
                    m_diffuseTexture = value;
                    UpdateTexture();
                    OnPropertyChanged(nameof(DiffuseTexture));
                }
            }
        }

        // State
        [Category("State")]
        [DisplayName("Selected")]
        [Description("Selection status.")]
        public bool Selected { get; set; } = false;

        // Transformation
        [Category("Transformation")]
        [DisplayName("Position")]
        [Description("Position of the model in world space.")]
        public Vector3 Position
        {
            get => m_position;
            set
            {
                if (m_position != value)
                {
                    m_position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        [Category("Transformation")]
        [DisplayName("Rotation")]
        [Description("Rotation of the model.")]
        public Vector3 Rotation
        {
            get => m_rotation;
            set
            {
                if (m_rotation != value)
                {
                    m_rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        [Category("Transformation")]
        [DisplayName("Scale")]
        [Description("Scale of the model.")]
        public float Scale
        {
            get => m_scale;
            set
            {
                if (m_scale != value)
                {
                    m_scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        public Models()
        {
        }

        public Models(ContentManager _content,
                        string _model,
                        string _texture,
                        string _effect,
                        Vector3 _position,
                        float _scale)
        {
            Create(_content, _model, _texture, _effect, _position, _scale);
        }

        public void Create(ContentManager _content,
                            string _model,
                            string _texture,
                            string _effect,
                            Vector3 _position,
                            float _scale)
        {
            m_content = _content;
            Mesh = _content.Load<Model>(_model);
            Mesh.Tag = _model;
            Texture = _content.Load<Texture>(_texture);
            Texture.Tag = _texture;
            Shader = _content.Load<Effect>(_effect);
            Shader.Tag = _effect;
            SetShader(Shader);
            m_position = _position;
            Scale = _scale;
        }

        public void SetShader(Effect _effect)
        {
            Shader = _effect;
            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Shader;
                }
            }
        }

        private void UpdateTexture()
        {
            if (m_content == null) return;
            string textureName = m_diffuseTexture.ToString();
            Texture = m_content.Load<Texture>(textureName);
        }

        public void Translate(Vector3 _translate, Camera _camera)
        {
            float distance = Vector3.Distance(_camera.Target, _camera.Position);
            Vector3 foward = _camera.Target - _camera.Position;
            foward.Normalize();
            Vector3 left = Vector3.Cross(foward, Vector3.Up);
            left.Normalize();
            Vector3 up = Vector3.Cross(left, foward);
            up.Normalize();
            Position += left * _translate.X * distance;
            Position += up * _translate.Y * distance;
            Position += foward * _translate.Z * 100f;
        }

        public void Rotate(Vector3 _rotate)
        {
            Rotation += _rotate;
        }

        public Matrix GetTransform()
        {
            return Matrix.CreateScale(Scale) *
                   Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                   Matrix.CreateTranslation(Position);
        }

        public void Render(Matrix _view, Matrix _projection)
        {
            Shader.Parameters["World"].SetValue(GetTransform());
            Shader.Parameters["WorldViewProjection"].SetValue(GetTransform() * _view * _projection);
            Shader.Parameters["Texture"].SetValue(Texture);
            Shader.Parameters["Tint"].SetValue(Selected);

            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                mesh.Draw();
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(Mesh.Tag.ToString());
            _stream.Write(Texture.Tag.ToString());
            _stream.Write(Shader.Tag.ToString());
            HelpSerialize.Vec3(_stream, Position);
            HelpSerialize.Vec3(_stream, Rotation);
            _stream.Write(Scale);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            string mesh = _stream.ReadString();
            string texture = _stream.ReadString();
            string shader = _stream.ReadString();
            Position = HelpDeserialize.Vec3(_stream);
            Rotation = HelpDeserialize.Vec3(_stream);
            Scale = _stream.ReadSingle();
            Create(_content, mesh, texture, shader, Position, Scale);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
