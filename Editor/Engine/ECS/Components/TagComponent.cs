namespace Editor.Engine.ECS.Components
{
    public class TagComponent : IComponent
    {
        public string Tag { get; set; }

        public TagComponent(string tag)
        {
            Tag = tag;
        }
    }
}
