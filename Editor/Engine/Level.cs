using Microsoft.Xna.Framework;
using Editor.Engine.Interfaces;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Editor.Editor;
using Microsoft.Xna.Framework.Audio;

namespace Editor.Engine
{
    internal class Level : ISerializable
    {
        // Accessors
        public Camera GetCamera() { return m_camera; }

        // Members
        private List<Models> m_models = new();
        private Camera m_camera = new(new Vector3(0, 400, 500), 16 / 9);
        private Light m_light = new() { Position = new(0, 400, -500), 
                                        Color = new(0.9f, 0.9f, 0.9f) };
        private Terrain m_terrain = null;

        public Level()
        {
        }

        public void LoadContent(GameEditor _game)
        {
            //m_terrain = new(_game.DefaultEffect, _game.DefaultHeightMap, _game.DefaultGrass, 200, _game.GraphicsDevice);
        }

        public void ClearSelectedModels()
        {
            foreach (var model in m_models)
            {
                model.Selected = false;
            }
            if (m_terrain != null)
            {
                m_terrain.Selected = false;
            }
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public List<ISelectable> GetSelectedModels()
        {
            List<ISelectable> models = new();
            foreach (var model in m_models)
            {
                if (model.Selected) models.Add(model);
            }
            if (m_terrain != null)
            {
                if (m_terrain.Selected) models.Add(m_terrain);
            }
            return models;
        }

        public void HandleTranslate()
        {
            InputController ic = InputController.Instance;
            Vector3 translate = Vector3.Zero;
            if (ic.IsKeyDown(Keys.Left)) translate.X += -10;
            if (ic.IsKeyDown(Keys.Right)) translate.X += 10;
            if (ic.IsKeyDown(Keys.Menu))
            {
                if (ic.IsKeyDown(Keys.Up)) translate.Z += 1;
                if (ic.IsKeyDown(Keys.Down)) translate.Z += -1;
            }
            else
            {
                if (ic.IsKeyDown(Keys.Up)) translate.Y += 10;
                if (ic.IsKeyDown(Keys.Down)) translate.Y += -10;
            }
            if (ic.IsButtonDown(MouseButtons.Middle))
            {
                Vector2 dir = ic.MousePosition - ic.LastPosition;
                translate.X = dir.X;
                translate.Y = -dir.Y;
            }

            if (ic.GetWheel() != 0)
            {
                translate.Z += ic.GetWheel() * 2;
            }

            if (translate != Vector3.Zero)
            {
                bool modelTranslate = false;
                foreach (Models model in m_models)
                {
                    if (model.Selected)
                    {
                        modelTranslate = true;
                        model.Translate(translate / 1000, m_camera);
                    }
                }
                if (!modelTranslate)
                {
                    m_camera.Translate(translate * 0.001f);
                }
            }
        }

        private void HandleRotate(float _delta)
        {
            InputController ic = InputController.Instance;
            if (ic.IsButtonDown(MouseButtons.Right) && (!ic.IsKeyDown(Keys.Menu)))
            {
                Vector2 dir = ic.MousePosition - ic.LastPosition;
                if (dir != Vector2.Zero)
                {
                    Vector3 movement = new Vector3(dir.Y, dir.X, 0) * _delta;
                    bool modelRotate = false;
                    foreach (Models model in m_models)
                    {
                        if (model.Selected)
                        {
                            modelRotate = true;
                            model.Rotate(movement);
                        }
                    }
                    if (!modelRotate)
                    {
                        m_camera.Rotate(movement);
                    }
                }
            }
        }
        private void HandleScale(float _delta)
        {
            InputController ic = InputController.Instance;
            if (ic.IsButtonDown(MouseButtons.Right) && (ic.IsKeyDown(Keys.Menu)))
            {
                float l = ic.MousePosition.X - ic.LastPosition.X;
                if (l != 0)
                {
                    l *= _delta;
                    foreach (Models model in m_models)
                    {
                        if (model.Selected)
                        {
                            model.Scale += l;
                        }
                    }
                }
            }
        }

        internal ISelectable HandlePick(bool _select = true)
        {
            float? f;
            Matrix transform = Matrix.Identity;
            InputController ic = InputController.Instance;
            if ((ic.IsButtonDown(MouseButtons.Left)) || (!_select))
            {
                Ray r = HelpMath.GetPickRay(ic.MousePosition, m_camera);
                foreach (Models model in m_models)
                {
                    if (_select) model.Selected = false;
                    transform = model.GetTransform();
                    foreach (ModelMesh mesh in model.Mesh.Meshes)
                    {
                        BoundingSphere s = mesh.BoundingSphere;
                        s.Transform(ref transform, out s);
                        f = r.Intersects(s);
                        if (f.HasValue)
                        {
                            f = HelpMath.PickTriangle(in mesh, ref r, ref transform);
                            if (f.HasValue)
                            {
                                if (!_select) return model;
                                model.Selected = true;
                            }
                        }
                    }
                }

                if (m_terrain != null)
                {
                    //Check terrain
                    transform = Matrix.Identity;
                    f = HelpMath.PickTriangle(in m_terrain, ref r, ref transform);
                    m_terrain.Selected = false;
                    if (f.HasValue)
                    {
                        if (!_select) return m_terrain;
                        m_terrain.Selected = true;
                    }
                }
            }
            return null;
        }

        private void HandleAudio()
        {
            foreach(Models m in m_models)
            {
                if((Models.SelectedDirty && m.Selected))
                {
                    var sfi = m.SoundEffects[(int)SoundEffectTypes.OnSelect];
                    if(sfi?.State == SoundState.Stopped)
                    {
                        sfi.Play();
                    }
                }
            }
        }

        public void Update(float _delta)
        {
            HandleTranslate();
            HandleRotate(_delta);
            HandleScale(_delta);
            HandlePick();
            HandleAudio();
        }

        public void Render()
        {
            Renderer r = Renderer.Instance;
            r.Camera = m_camera;
            r.Light = m_light;
            foreach (Models m in m_models)
            {
                r.Render(m);
            }
            if (m_terrain != null)
            {
                r.Render(m_terrain);
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(m_models.Count);
            foreach (var model in m_models)
            {
                model.Serialize(_stream);
            }
            m_camera.Serialize(_stream);
        }

        public void Deserialize(BinaryReader _stream, GameEditor _game)
        {
            int modelCount = _stream.ReadInt32();
            for (int count = 0; count < modelCount; count++)
            {
                Models m = new();
                m.Deserialize(_stream, _game);
                m_models.Add(m);
            }
            m_camera.Deserialize(_stream, _game);
        }

        public override string ToString()
        {
            string s = string.Empty;
            foreach (Models m in m_models)
            {
                if (m.Selected)
                {
                    s += "\nModel: Pos: " + m.Position.ToString() + " Rot: " + m.Rotation.ToString();
                }
            }
            return m_camera.ToString() + s;
        }
    }
}
