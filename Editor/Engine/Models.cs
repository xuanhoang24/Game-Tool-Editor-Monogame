using Editor.Editor;
using Editor.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Editor.Engine
{
    class Models : ISerializable, IRenderable, ISelectable, ISoundEmitter
    {
        // Accessors
        public Model Mesh { get; set; }
        public Material Material { get; private set; }
        public SFXInstance[] SoundEffects { get; private set; }
        public Vector3 Position { get => m_position; set { m_position = value; } }
        public Vector3 Rotation { get => m_rotation; set { m_rotation = value; } }
        public float Scale { get; set; }
        public bool Selected
        {
            get { return m_selected; }
            set
            {
                if (m_selected != value)
                {
                    m_selected = value;
                    SelectedDirty = true;
                }
            }
        }
        public string Name { get; set; }
        public static bool SelectedDirty { get; set; } = false;

        // Members
        private Vector3 m_position;
        private Vector3 m_rotation;
        private bool m_selected;

        public Models()
        {
        }

        public Models(GameEditor _game,
                        string _model,
                        string _texture,
                        string _effect,
                        Vector3 _position,
                        float _scale)
        {
            Create(_game, _model, _texture, _effect, _position, _scale);
        }

        public void Create(GameEditor _game,
                            string _model,
                            string _texture,
                            string _effect,
                            Vector3 _position,
                            float _scale)
        {
            /*string fileName = Path.Combine(_game.Project.Folder, _game.Project.ContentFolder, _game.Project.AssetFolder, _model);
            Mesh = _game.Content.Load<Model>(fileName);*/
            Mesh = _game.Content.Load<Model>(_model);
            Mesh.Tag = _model;
            Name = _model;
            Material = new Material();
            SetTexture(_game, _texture);
            SetShader(_game, _effect);
            m_position = _position;
            Scale = _scale;
            SoundEffects ??= new SFXInstance[Enum.GetNames(typeof(SoundEffectTypes)).Length];
        }

        public void SetTexture(GameEditor _game, string _texture)
        {
            if(_texture == "DefaultTexture")
            {
                Material.Diffuse = _game.DefaultTexture;
            }
            else
            {
                /*string fileName = Path.Combine(_game.Project.Folder, _game.Project.ContentFolder, _game.Project.AssetFolder, _texture);
                Material.Diffuse = _game.Content.Load<Texture>(fileName);*/
                Material.Diffuse = _game.Content.Load<Texture>(_texture);
            }
            Material.Diffuse.Tag = _texture;
        }

        public void SetShader(Effect _effect)
        {
            Material.Effect = _effect;
            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Material.Effect;
                }
            }
        }

        public void SetShader(GameEditor _game, string _effect)
        {
            if (_effect == "DefaultEffect")
            {
                Material.Effect = _game.DefaultEffect; ;
            }
            else
            {
                /*string fileName = Path.Combine(_game.Project.Folder, _game.Project.ContentFolder, _game.Project.AssetFolder, _effect);
                Material.Effect = _game.Content.Load<Effect>(fileName);*/
                Material.Effect = _game.Content.Load<Effect>(_effect);
            }
            Material.Effect.Tag = _effect;
            SetShader(Material.Effect);
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

        public void Render()
        {
            foreach (ModelMesh mesh in Mesh.Meshes)
            {
                mesh.Draw();
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(Mesh.Tag.ToString());
            _stream.Write(Material.Diffuse.ToString());
            _stream.Write(Material.Effect.ToString());
            HelpSerialize.Vec3(_stream, Position);
            HelpSerialize.Vec3(_stream, Rotation);
            _stream.Write(Scale);
            _stream.Write(Selected);
            _stream.Write(Name);
            _stream.Write(SoundEffects.Length);
            foreach (var sfi in SoundEffects)
            {
                if (sfi == null)
                {
                    _stream.Write("empty!");
                }
                else
                {
                    _stream.Write(sfi.Name);
                }
            }
        }

        public void Deserialize(BinaryReader _stream, GameEditor _game)
        {
            string mesh = _stream.ReadString();
            string texture = _stream.ReadString();
            string shader = _stream.ReadString();
            Position = HelpDeserialize.Vec3(_stream);
            Rotation = HelpDeserialize.Vec3(_stream);
            Scale = _stream.ReadSingle();
            Selected = _stream.ReadBoolean();
            Name = _stream.ReadString();

            int sfxCount = _stream.ReadInt32();
            SoundEffects = new SFXInstance[sfxCount];
            for (int count = 0; count < sfxCount; count++)
            {
                string assetName = _stream.ReadString();
                if(assetName !="empty!")
                {
                    SoundEffects[count] = SFXInstance.Create(_game, assetName);
                }
            }

            Create(_game, mesh, texture, shader, Position, Scale);
        }
    }
}
