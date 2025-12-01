namespace Editor.Engine.ECS.Components
{
    public class OrbitComponent : IComponent
    {
        public int ParentEntityId { get; set; }
        public float OrbitSpeed { get; set; }
        public float OrbitAngle { get; set; }
        public float OrbitRadius { get; set; }

        public OrbitComponent(int parentId, float speed, float angle, float radius)
        {
            ParentEntityId = parentId;
            OrbitSpeed = speed;
            OrbitAngle = angle;
            OrbitRadius = radius;
        }
    }
}
