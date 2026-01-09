using Editor.Editor;
using Editor.Engine.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Editor.Engine
{
    internal class Group : ISerializable
    {
        public List<Models> GroupModels { get; private set; } = new List<Models>();
        public string Name { get; set; } = "Group";
        public bool Selected { get; set; } = false;

        public Group()
        {
        }

        public Group(List<Models> models, string name = "Group")
        {
            Name = name;
            GroupModels.AddRange(models);
        }
        public List<Models> Ungroup()
        {
            var ungroupedModels = new List<Models>(GroupModels);
            GroupModels.Clear();
            Selected = false;
            return ungroupedModels;
        }

        public void RemoveModels(List<Models> modelsToRemove)
        {
            foreach (var model in modelsToRemove)
            {
                GroupModels.Remove(model);
            }
        }

        public void Serialize(BinaryWriter stream)
        {
            stream.Write(Name);
            stream.Write(GroupModels.Count);
            foreach (var model in GroupModels)
            {
                model.Serialize(stream);
            }
        }

        public void Deserialize(BinaryReader stream, GameEditor game)
        {
            Name = stream.ReadString();
            int modelCount = stream.ReadInt32();
            GroupModels.Clear();
            
            for (int i = 0; i < modelCount; i++)
            {
                var model = new Models();
                model.Deserialize(stream, game);
                GroupModels.Add(model);
            }
        }
    }
}