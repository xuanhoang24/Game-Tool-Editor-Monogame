using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Editor.Engine.Interfaces;
using Editor.Engine.ECS;
using Editor.Engine.ECS.Components;
using System.IO;
using System.Linq;
using System;
using System.Windows.Forms;


namespace Editor.Engine
{
    internal class Level : ISerializable
    {
        // Members
        private World m_world = new();
        private Camera m_camera = new(new Vector3(0, 0, 300), 16 / 9);
        private static Random m_random = new Random();
        private ContentManager m_content;

        // Accessors
        public Camera GetCamera() { return m_camera; }
        public Entity GetSun() { return m_world.GetEntities().FirstOrDefault(e => e.HasComponent<TagComponent>() && e.GetComponent<TagComponent>().Tag == "Sun"); }
        public Entity[] GetPlanets() { return m_world.GetEntities().Where(e => e.HasComponent<TagComponent>() && e.GetComponent<TagComponent>().Tag == "World").ToArray(); }

        public Level()
        {
        }

        public void LoadContent(ContentManager _content)
        {
            m_content = _content;
            m_world.SetCamera(m_camera);
        }

        private Entity CreateEntity(string modelName, string textureName, string shaderName, Vector3 position, float scale)
        {
            var entity = m_world.CreateEntity(modelName);
            
            var model = m_content.Load<Model>(modelName);
            var texture = m_content.Load<Texture>(textureName);
            var shader = m_content.Load<Effect>(shaderName).Clone();

            entity.AddComponent(new TransformComponent(position, scale));
            entity.AddComponent(new MeshComponent(model, texture, shader));
            entity.AddComponent(new TagComponent(modelName));
            
            return entity;
        }

        public void AddSun(ContentManager _content)
        {
            if (GetSun() != null)
            {
                MessageBox.Show("A Sun already exists in this level!",
                                "Duplicate Sun",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            var sun = CreateEntity("Sun", "SunDiffuse", "MyShader", Vector3.Zero, 2.0f);
            sun.AddComponent(new RotationComponent(0f, 0.005f, 0f));
        }

        public void AddPlanet(ContentManager _content)
        {
            int planetCount = GetPlanets().Length;
            if (planetCount >= 5)
            {
                MessageBox.Show("You can only have up to 5 planets in this level!",
                                "Planet Limit Reached",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            Vector3 position = new Vector3(
                GenerateRandomFloat(-150, 151),
                GenerateRandomFloat(-90, 91),
                0f
            );

            var planet = CreateEntity("World", "WorldDiffuse", "MyShader", position, 0.75f);
            planet.AddComponent(new RotationComponent(0f, GenerateRandomFloat(0.02f, 0.03f), 0f));

            var sun = GetSun();
            if (sun != null && sun.HasComponent<TransformComponent>())
            {
                var sunTransform = sun.GetComponent<TransformComponent>();
                float orbitAngle = (float)Math.Atan2(position.X - sunTransform.Position.X, position.Z - sunTransform.Position.Z);
                float orbitRadius = Vector3.Distance(position, sunTransform.Position);
                
                planet.AddComponent(new OrbitComponent(sun.Id, GenerateRandomFloat(0.001f, 0.002f), orbitAngle, orbitRadius));
            }
        }

        public void AddMoon(ContentManager _content)
        {
            var planets = GetPlanets();
            if (planets.Length == 0)
            {
                MessageBox.Show("You need at least one planet to add a moon!",
                                "No Planets",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            // Pick a random planet to add moon
            var planet = planets[m_random.Next(planets.Length)];
            
            if (!planet.HasComponent<TransformComponent>()) return;
            
            var planetTransform = planet.GetComponent<TransformComponent>();
            
            // Random angle and distance for moon
            float randomAngle = GenerateRandomFloat(0f, (float)(Math.PI * 2));
            float distance = GenerateRandomFloat(15f, 30f);
            
            Vector3 moonPos = new Vector3(
                planetTransform.Position.X + (float)(Math.Cos(randomAngle) * distance),
                planetTransform.Position.Y,
                planetTransform.Position.Z + (float)(Math.Sin(randomAngle) * distance)
            );

            float scale = GenerateRandomFloat(0.2f, 0.4f);
            var moon = CreateEntity("Moon", "MoonDiffuse", "MyShader", moonPos, scale);
            moon.AddComponent(new RotationComponent(0f, GenerateRandomFloat(0.005f, 0.01f), 0f));

            float orbitAngle = (float)Math.Atan2(moonPos.X - planetTransform.Position.X, moonPos.Z - planetTransform.Position.Z);
            float orbitRadius = Vector3.Distance(moonPos, planetTransform.Position);
            moon.AddComponent(new OrbitComponent(planet.Id, GenerateRandomFloat(0.01f, 0.02f), orbitAngle, orbitRadius));
        }

        public static float GenerateRandomFloat(float min, float max)
        {
            return (float)(min + (m_random.NextDouble() * (max - min)));
        }

        public void Update(GameTime gameTime)
        {
            // Update rotation and orbit systems
            foreach (var entity in m_world.GetEntities())
            {
                if (entity.HasComponent<RotationComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var rotation = entity.GetComponent<RotationComponent>();
                    var transform = entity.GetComponent<TransformComponent>();
                    transform.Rotation += rotation.RotationSpeed;
                }
            }

            foreach (var entity in m_world.GetEntities())
            {
                if (entity.HasComponent<OrbitComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var orbit = entity.GetComponent<OrbitComponent>();
                    var transform = entity.GetComponent<TransformComponent>();
                    
                    var parent = m_world.GetEntity(orbit.ParentEntityId);
                    if (parent != null && parent.HasComponent<TransformComponent>())
                    {
                        var parentTransform = parent.GetComponent<TransformComponent>();
                        orbit.OrbitAngle += orbit.OrbitSpeed;

                        transform.Position = new Vector3(
                            (float)(Math.Cos(orbit.OrbitAngle) * orbit.OrbitRadius) + parentTransform.Position.X,
                            transform.Position.Y,
                            (float)(Math.Sin(orbit.OrbitAngle) * orbit.OrbitRadius) + parentTransform.Position.Z
                        );
                    }
                }
            }
        }

        public void Render()
        {
            var camera = m_camera;
            if (camera == null) return;

            foreach (var entity in m_world.GetEntities())
            {
                if (entity.HasComponent<MeshComponent>() && entity.HasComponent<TransformComponent>())
                {
                    var mesh = entity.GetComponent<MeshComponent>();
                    var transform = entity.GetComponent<TransformComponent>();

                    Matrix worldMatrix = transform.GetWorldMatrix();
                    mesh.Shader.Parameters["World"].SetValue(worldMatrix);
                    mesh.Shader.Parameters["WorldViewProjection"].SetValue(worldMatrix * camera.View * camera.Projection);
                    mesh.Shader.Parameters["Texture"].SetValue(mesh.Texture);

                    foreach (ModelMesh modelMesh in mesh.Model.Meshes)
                    {
                        foreach (ModelMeshPart part in modelMesh.MeshParts)
                        {
                            part.Effect = mesh.Shader;
                            modelMesh.Draw();
                        }
                    }
                }
            }
        }

        public void Serialize(BinaryWriter _stream)
        {
            m_camera.Serialize(_stream);
        }

        public void Deserialize(BinaryReader _stream, ContentManager _content)
        {
            m_camera.Deserialize(_stream, _content);
        }
    }
}
