using Editor.Editor;
using Editor.Engine.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Editor.Engine
{
    internal class Group : ISerializable
    {
        public List<Models> GroupModels { get; private set; } = new List<Models>();
        public List<Group> NestedGroups { get; private set; } = new List<Group>();
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

        public Group(List<Models> models, List<Group> nestedGroups, string name = "Group")
        {
            Name = name;
            GroupModels.AddRange(models);
            NestedGroups.AddRange(nestedGroups);
        }
        public List<Models> Ungroup()
        {
            var ungroupedModels = new List<Models>(GroupModels);
            
            // Add models from nested groups
            foreach (var nestedGroup in NestedGroups)
            {
                ungroupedModels.AddRange(nestedGroup.Ungroup());
            }
            
            GroupModels.Clear();
            NestedGroups.Clear();
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

        public void AddModel(Models model)
        {
            if (model != null && !GroupModels.Contains(model))
            {
                GroupModels.Add(model);
            }
        }

        public void AddNestedGroup(Group group)
        {
            if (group != null && !NestedGroups.Contains(group))
            {
                NestedGroups.Add(group);
            }
        }

        public void RemoveNestedGroup(Group group)
        {
            NestedGroups.Remove(group);
        }

        public Group FindGroupContaining(Models model)
        {
            // Check direct models
            if (GroupModels.Contains(model))
            {
                return this;
            }
            
            // Check nested groups
            foreach (var nestedGroup in NestedGroups)
            {
                var found = nestedGroup.FindGroupContaining(model);
                if (found != null)
                {
                    return found;
                }
            }
            
            return null;
        }

        public void Serialize(BinaryWriter stream)
        {
            stream.Write(Name);
            stream.Write(GroupModels.Count);
            foreach (var model in GroupModels)
            {
                model.Serialize(stream);
            }
            
            // Serialize nested groups
            stream.Write(NestedGroups.Count);
            foreach (var nestedGroup in NestedGroups)
            {
                nestedGroup.Serialize(stream);
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
            
            // Deserialize nested groups
            try
            {
                int nestedGroupCount = stream.ReadInt32();
                NestedGroups.Clear();
                
                for (int i = 0; i < nestedGroupCount; i++)
                {
                    var nestedGroup = new Group();
                    nestedGroup.Deserialize(stream, game);
                    NestedGroups.Add(nestedGroup);
                }
            }
            catch
            {
            }
        }
    }
}