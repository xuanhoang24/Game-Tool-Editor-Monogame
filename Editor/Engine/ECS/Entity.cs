using System;
using System.Collections.Generic;

namespace Editor.Engine.ECS
{
    public class Entity
    {
        public int Id { get; }
        public string Name { get; set; }
        private Dictionary<Type, IComponent> m_components = new();

        public Entity(int id, string name = "Entity")
        {
            Id = id;
            Name = name;
        }

        public void AddComponent<T>(T component) where T : IComponent
        {
            m_components[typeof(T)] = component;
        }

        public T GetComponent<T>() where T : IComponent
        {
            return m_components.TryGetValue(typeof(T), out var component) ? (T)component : default;
        }

        public bool HasComponent<T>() where T : IComponent
        {
            return m_components.ContainsKey(typeof(T));
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            m_components.Remove(typeof(T));
        }
    }
}
