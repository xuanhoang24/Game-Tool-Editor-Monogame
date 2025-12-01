using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Editor.Engine.Interfaces;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Forms;


namespace Editor.Engine
{
    internal class Level : ISerializable
    {
        // Members
        private List<Models> m_models = new();
        private Camera m_camera = new(new Vector3(0, 0, 300), 16 / 9);
        private static Random m_random = new Random();

        // Accessors
        public Camera GetCamera() { return m_camera; }
        public Models GetSun() { return m_models.Find(m => m.Mesh.Tag.ToString() == "Sun"); }
        public List<Models> GetPlanets() { return m_models.Where(m => m.Mesh.Tag.ToString() == "World").ToList(); }

        public Level()
        {
        }

        public void LoadContent(ContentManager _content)
        {
            Models teapot = new(_content, "Teapot", "Metal", "MyShader", Vector3.Zero, 1.0f);
            AddModel(teapot);
        }

        public void AddModel(Models _model)
        {
            m_models.Add(_model);
        }

        public void AddSun(ContentManager _content)
        {
            if (m_models.Any(m => m.Mesh.Tag.ToString() == "Sun"))
            {
                MessageBox.Show("A Sun already exists in this level!",
                                "Duplicate Sun",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }
            else
            {
                Models sun = new(_content, "Sun", "SunDiffuse", "MyShader", Vector3.Zero, 2.0f);
                sun.SetRotationSpeed(0f, 0.005f, 0f);
                AddModel(sun);
            }
        }

        public void AddPlanet(ContentManager _content)
        {
            int planetCount = m_models.Count(m => m.Mesh.Tag.ToString() == "World");
            if (planetCount == 5)
            {
                MessageBox.Show("You can only have up to 5 planets in this level!",
                                "Planet Limit Reached",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            else
            {
                // Generate random position
                Vector3 position = new Vector3(
                                        GenerateRandomFloat(-150, 151),
                                        GenerateRandomFloat(-90, 91),
                                        0f
                                        );

                Models planet = new(_content, "World", "WorldDiffuse", "MyShader", position, 0.75f);
                planet.SetRotationSpeed(0f, GenerateRandomFloat(0.02f, 0.03f), 0f);

                Models sun = GetSun();
                if (sun != null)
                {
                    // Setup orbit
                    planet.OrbitParent = GetSun();
                    planet.OrbitSpeed = GenerateRandomFloat(0.001f, 0.002f);
                    planet.OrbitAngle = (float)Math.Atan2(position.X - sun.Position.X, position.Z - sun.Position.Z);
                    planet.OrbitRadius = Vector3.Distance(position, sun.Position);
                }
                AddModel(planet);
            }
        }

        public void AddMoon(ContentManager _content)
        {
            foreach (Models _planet in GetPlanets())
            {
                Vector3 moonPos = new Vector3(
                                        _planet.Position.X + 20f,
                                        _planet.Position.Y,
                                        _planet.Position.Z
                                        );

                float _scale = GenerateRandomFloat(0.2f, 0.4f);
                Models moon = new(_content, "Moon", "MoonDiffuse", "MyShader", moonPos, _scale);
                moon.SetRotationSpeed(0f, GenerateRandomFloat(0.005f, 0.01f), 0f);

                moon.OrbitParent = _planet;
                moon.OrbitSpeed = GenerateRandomFloat(0.01f, 0.02f);
                moon.OrbitAngle = (float)(Math.Atan2(moonPos.X - _planet.Position.X, moonPos.Z - _planet.Position.Z));
                moon.OrbitRadius = Vector3.Distance(moonPos, _planet.Position);
                AddModel(moon);
            }
        }

        public static float GenerateRandomFloat(float min, float max)
        {
            return (float)(min + (m_random.NextDouble() * (max - min)));
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
    }
}
