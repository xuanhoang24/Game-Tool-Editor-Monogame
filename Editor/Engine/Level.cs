using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Editor.Engine.Interfaces;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Editor.Engine
{
    internal class Level : ISerializable
    {
        // Accessors
        public Camera GetCamera() { return m_camera; }

        // Members
        private List<Models> m_models = new();
        private Camera m_camera = new(new Vector3(0, 2, 2), 16 / 9);

        public Level()
        {
        }

        public void LoadContent(ContentManager _content)
        {
            Models teapot = new(_content, "Teapot", "Metal", "MyShader", Vector3.Zero, 1.0f);
            AddModel(teapot);
            teapot = new(_content, "Teapot", "Metal", "MyShader", new Vector3(1, 0, 0), 1.0f);
            AddModel(teapot);
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
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
                translate.X = -dir.X;
                translate.Y = dir.Y;
            }

            if (ic.GetWheel() != 0)
            {
                translate.Z += ic.GetWheel() * 2;
            }

            if (translate != Vector3.Zero)
            {
                m_camera.Translate(translate * 0.001f);
            }


        }

        private void HandleRotate(float _delta)
        {
            InputController ic = InputController.Instance;
            if (ic.IsButtonDown(MouseButtons.Right))
            {
                Vector2 dir = ic.MousePosition - ic.LastPosition;
                if (dir != Vector2.Zero)
                {
                    Vector3 movement = new Vector3(dir.Y, dir.X, 0) * _delta;
                    m_camera.Rotate(movement);
                }
            }
        }

        private void HandlePick()
        {
            InputController ic = InputController.Instance;
            if (ic.IsButtonDown(MouseButtons.Left))
            {
                Ray r = ic.GetPickRay(m_camera);
                foreach (Models model in m_models)
                {
                    model.Selected = false;
                    foreach (ModelMesh mesh in model.Mesh.Meshes)
                    {
                        BoundingSphere s = mesh.BoundingSphere;
                        s = s.Transform(model.GetTransform());
                        float? f = r.Intersects(s);
                        if (f.HasValue)
                        {
                            model.Selected = true;
                        }
                    }
                }
            }
        }

        public void Update(float _delta)
        {
            HandleTranslate();
            HandleRotate(_delta);
            HandlePick();
        }

        public void Render()
        {
            foreach (Models m in m_models)
            {
                m.Render(m_camera.View, m_camera.Projection);
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

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            int modelCount = _stream.ReadInt32();
            for (int count = 0; count < modelCount; count++)
            {
                Models m = new();
                m.Deserialize(_stream, _content);
                m_models.Add(m);
            }
            m_camera.Deserialize(_stream, _content);
        }

        public override string ToString()
        {
            return m_camera.ToString();
        }
    }
}
