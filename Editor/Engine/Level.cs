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
        public Terrain GetTerrain() { return m_terrain; }
        public Light GetLight() { return m_light; }

        // Members
        private List<Models> m_models = new();
        private List<Group> m_groups = new();
        private Camera m_camera = new(new Vector3(0, 400, 500), 16 / 9);
        private Light m_light = new() { Position = new(0, 400, -500), 
                                        Color = new(0.9f, 0.9f, 0.9f) };
        private Terrain m_terrain = null;

        public Level()
        {
        }

        public void LoadContent(GameEditor _game)
        {
            m_terrain = new(_game.DefaultEffect, 
                        _game.DefaultHeightMap, 
                        _game.DefaultGrass, 200, 
                        _game.GraphicsDevice);
        }

        public void ClearSelectedModels()
        {
            foreach (var model in m_models)
            {
                model.Selected = false;
            }
            foreach (var group in m_groups)
            {
                ClearGroupSelection(group);
            }
            if (m_terrain != null)
            {
                m_terrain.Selected = false;
            }
        }

        private void ClearGroupSelection(Group group)
        {
            group.Selected = false;
            foreach (var model in group.GroupModels)
            {
                model.Selected = false;
            }
            foreach (var nestedGroup in group.NestedGroups)
            {
                ClearGroupSelection(nestedGroup);
            }
        }

        public List<Models> GetModelsList()
        {
            return m_models;
        }

        public List<Group> GetGroupsList()
        {
            return m_groups;
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public void AddGroup(Group _group)
        {
            m_groups.Add(_group);
        }

        public void RemoveGroup(Group _group)
        {
            m_groups.Remove(_group);
        }

        public Group FindGroupContaining(Models model)
        {
            foreach (var group in m_groups)
            {
                var found = group.FindGroupContaining(model);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public void ApplyTextureToTarget(GameEditor game, object target, string textureName)
        {
            if (target is Models model)
            {
                model.SetTexture(game, textureName);
            }
        }

        public void ApplyShaderToTarget(GameEditor game, object target, string shaderName)
        {
            if (target is Models model)
            {
                model.SetShader(game, shaderName);
            }
        }

        public List<ISelectable> GetSelectedModels()
        {
            List<ISelectable> models = new();
            foreach (var model in m_models)
            {
                if (model.Selected) models.Add(model);
            }
            foreach (var group in m_groups)
            {
                GetSelectedModelsFromGroup(group, models);
            }
            if (m_terrain != null)
            {
                if (m_terrain.Selected) models.Add(m_terrain);
            }
            return models;
        }

        private void GetSelectedModelsFromGroup(Group group, List<ISelectable> models)
        {
            foreach (var model in group.GroupModels)
            {
                if (model.Selected) models.Add(model);
            }
            foreach (var nestedGroup in group.NestedGroups)
            {
                GetSelectedModelsFromGroup(nestedGroup, models);
            }
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
                foreach (Group group in m_groups)
                {
                    if (HandleTranslateInGroup(group, translate))
                    {
                        modelTranslate = true;
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
                    foreach (Group group in m_groups)
                    {
                        if (HandleRotateInGroup(group, movement))
                        {
                            modelRotate = true;
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
                    foreach (Group group in m_groups)
                    {
                        HandleScaleInGroup(group, l);
                    }
                }
            }
        }

        private bool HandleTranslateInGroup(Group group, Vector3 translate)
        {
            bool modelTranslated = false;
            foreach (Models model in group.GroupModels)
            {
                if (model.Selected)
                {
                    modelTranslated = true;
                    model.Translate(translate / 1000, m_camera);
                }
            }
            foreach (Group nestedGroup in group.NestedGroups)
            {
                if (HandleTranslateInGroup(nestedGroup, translate))
                {
                    modelTranslated = true;
                }
            }
            return modelTranslated;
        }

        private bool HandleRotateInGroup(Group group, Vector3 movement)
        {
            bool modelRotated = false;
            foreach (Models model in group.GroupModels)
            {
                if (model.Selected)
                {
                    modelRotated = true;
                    model.Rotate(movement);
                }
            }
            foreach (Group nestedGroup in group.NestedGroups)
            {
                if (HandleRotateInGroup(nestedGroup, movement))
                {
                    modelRotated = true;
                }
            }
            return modelRotated;
        }

        private void HandleScaleInGroup(Group group, float scale)
        {
            foreach (Models model in group.GroupModels)
            {
                if (model.Selected)
                {
                    model.Scale += scale;
                }
            }
            foreach (Group nestedGroup in group.NestedGroups)
            {
                HandleScaleInGroup(nestedGroup, scale);
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

                foreach (Group group in m_groups)
                {
                    HandlePickInGroup(group, r, _select);
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

        private ISelectable HandlePickInGroup(Group group, Ray r, bool _select)
        {
            Matrix transform;
            float? f;
            
            foreach (Models model in group.GroupModels)
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
            
            foreach (Group nestedGroup in group.NestedGroups)
            {
                var result = HandlePickInGroup(nestedGroup, r, _select);
                if (result != null) return result;
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
                    if(sfi?.Instance.State == SoundState.Stopped)
                    {
                        sfi.Instance.Play();
                    }
                }
            }
            foreach(Group group in m_groups)
            {
                HandleAudioInGroup(group);
            }
        }

        private void HandleAudioInGroup(Group group)
        {
            foreach(Models m in group.GroupModels)
            {
                if((Models.SelectedDirty && m.Selected))
                {
                    var sfi = m.SoundEffects[(int)SoundEffectTypes.OnSelect];
                    if(sfi?.Instance.State == SoundState.Stopped)
                    {
                        sfi.Instance.Play();
                    }
                }
            }
            foreach(Group nestedGroup in group.NestedGroups)
            {
                HandleAudioInGroup(nestedGroup);
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
            foreach (Group g in m_groups)
            {
                RenderGroup(g, r);
            }
            if (m_terrain != null)
            {
                r.Render(m_terrain);
            }
        }

        private void RenderGroup(Group group, Renderer renderer)
        {
            foreach (Models m in group.GroupModels)
            {
                renderer.Render(m);
            }
            foreach (Group nestedGroup in group.NestedGroups)
            {
                RenderGroup(nestedGroup, renderer);
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            _stream.Write(m_models.Count);
            foreach (var model in m_models)
            {
                model.Serialize(_stream);
            }
            _stream.Write(m_groups.Count);
            foreach (var group in m_groups)
            {
                group.Serialize(_stream);
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
            
            // Read groups
            try
            {
                int groupCount = _stream.ReadInt32();
                for (int count = 0; count < groupCount; count++)
                {
                    Group g = new();
                    g.Deserialize(_stream, _game);
                    m_groups.Add(g);
                }
            }
            catch
            {
                // Save files without groups - ignore
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
