using Editor.Engine.ECS.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Engine.ECS
{
    public class World
    {
        private List<Entity> m_entities = new();
        private List<ISystem> m_systems = new();
        private int m_nextEntityId = 0;
        private Camera m_camera;

        public World()
        {
            // Register systems
            m_systems.Add(new RotationSystem());
            m_systems.Add(new OrbitSystem());
            m_systems.Add(new RenderSystem());
        }

        public Entity CreateEntity(string name = "Entity")
        {
            var entity = new Entity(m_nextEntityId++, name);
            m_entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            m_entities.Remove(entity);
        }

        public Entity GetEntity(int id)
        {
            return m_entities.FirstOrDefault(e => e.Id == id);
        }

        public List<Entity> GetEntities()
        {
            return m_entities;
        }

        public void SetCamera(Camera camera)
        {
            m_camera = camera;
        }

        public Camera GetCamera()
        {
            return m_camera;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var system in m_systems)
            {
                system.Update(this, gameTime);
            }
        }
    }
}
