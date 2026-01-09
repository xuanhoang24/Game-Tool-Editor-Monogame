using Editor.Editor;
using Editor.Engine;
using Editor.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Editor
{
    public class SceneTreeView : TreeView
    {
        #region Private Fields
        private GameEditor m_game;
        private Level m_currentLevel;
        private Dictionary<TreeNode, object> m_nodeObjectMap = new();
        private Dictionary<object, TreeNode> m_objectNodeMap = new();
        private HashSet<TreeNode> m_selectedNodes = new();
        private TreeNode m_lastSelectedNode = null;
        private List<Models> m_clipboard = new();
        #endregion

        #region Events
        public event Action<object> OnObjectSelected;
        public event Action<List<object>> OnMultipleObjectsSelected;
        public event Action<object> OnObjectRenamed;
        #endregion

        #region Constructor and Initialization
        public SceneTreeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configure tree view appearance
            ShowLines = true;
            ShowPlusMinus = true;
            ShowRootLines = true;
            HideSelection = false;
            LabelEdit = true;
            AllowDrop = true;
            TabStop = true;
            
            // Event handlers for multi-selection
            BeforeSelect += OnBeforeSelect;
            MouseDown += OnMouseDown;
            AfterLabelEdit += OnAfterLabelEdit;
            MouseClick += OnMouseClick;
            DragOver += OnDragOver;
            DragDrop += OnDragDrop;
            ItemDrag += OnItemDrag;
        }

        public void Initialize(GameEditor game)
        {
            m_game = game;
            RefreshTree();
        }
        #endregion

        #region Tree Management
        public void RefreshTree()
        {
            if (m_game?.Project?.CurrentLevel == null) return;

            m_currentLevel = m_game.Project.CurrentLevel;
            
            BeginUpdate();
            Nodes.Clear();
            m_nodeObjectMap.Clear();
            m_objectNodeMap.Clear();

            // Create root level node
            TreeNode levelNode = new TreeNode("Level") { Tag = "Level" };
            Nodes.Add(levelNode);

            // Add camera
            TreeNode cameraNode = new TreeNode("Camera");
            levelNode.Nodes.Add(cameraNode);
            m_nodeObjectMap[cameraNode] = m_currentLevel.GetCamera();
            m_objectNodeMap[m_currentLevel.GetCamera()] = cameraNode;

            // Add light
            TreeNode lightNode = new TreeNode("Light");
            levelNode.Nodes.Add(lightNode);
            m_nodeObjectMap[lightNode] = m_currentLevel.GetLight();
            m_objectNodeMap[m_currentLevel.GetLight()] = lightNode;

            // Add terrain if exists
            if (m_currentLevel.GetTerrain() != null)
            {
                TreeNode terrainNode = new TreeNode("Terrain");
                levelNode.Nodes.Add(terrainNode);
                m_nodeObjectMap[terrainNode] = m_currentLevel.GetTerrain();
                m_objectNodeMap[m_currentLevel.GetTerrain()] = terrainNode;
            }

            // Add groups container
            TreeNode groupsNode = new TreeNode("Groups") { Tag = "GroupsContainer" };
            levelNode.Nodes.Add(groupsNode);

            // Add individual groups
            var groups = m_currentLevel.GetGroupsList();
            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                string groupName = !string.IsNullOrEmpty(group.Name) ? group.Name : $"Group_{i}";
                
                TreeNode groupNode = new TreeNode(groupName);
                groupsNode.Nodes.Add(groupNode);
                m_nodeObjectMap[groupNode] = group;
                m_objectNodeMap[group] = groupNode;

                // Add models within the group as child nodes
                foreach (var model in group.GroupModels)
                {
                    string modelName = !string.IsNullOrEmpty(model.Name) ? model.Name : $"Model_{model.GetHashCode()}";
                    TreeNode modelNode = new TreeNode(modelName);
                    groupNode.Nodes.Add(modelNode);
                    m_nodeObjectMap[modelNode] = model;
                    m_objectNodeMap[model] = modelNode;
                }
            }

            // Add models container
            TreeNode modelsNode = new TreeNode("Models") { Tag = "ModelsContainer" };
            levelNode.Nodes.Add(modelsNode);

            // Add individual models
            var models = m_currentLevel.GetModelsList();
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                string modelName = !string.IsNullOrEmpty(model.Name) ? model.Name : $"Model_{i}";
                
                TreeNode modelNode = new TreeNode(modelName);
                modelsNode.Nodes.Add(modelNode);
                m_nodeObjectMap[modelNode] = model;
                m_objectNodeMap[model] = modelNode;
            }

            levelNode.Expand();
            groupsNode.Expand();
            modelsNode.Expand();
            EndUpdate();
        }
        #endregion

        #region Keyboard Shortcuts
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Delete:
                    HandleDeleteKey();
                    return true;
                    
                case Keys.Control | Keys.D:
                    HandleDuplicateKey();
                    return true;
                    
                case Keys.Control | Keys.C:
                    HandleCopyKey();
                    return true;
                    
                case Keys.Control | Keys.V:
                    HandlePasteKey();
                    return true;
                    
                case Keys.Control | Keys.A:
                    HandleSelectAllKey();
                    return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void HandleDeleteKey()
        {
            var selectedObjects = GetSelectedObjects();
            if (selectedObjects.Count > 0)
            {
                var models = selectedObjects.OfType<Models>().ToList();
                if (models.Count > 0)
                {
                    if (models.Count == 1)
                        DeleteObject(models[0]);
                    else
                        DeleteMultipleObjects(models);
                }
            }
        }

        private void HandleDuplicateKey()
        {
            var selectedObjects = GetSelectedObjects();
            if (selectedObjects.Count > 0)
            {
                var models = selectedObjects.OfType<Models>().ToList();
                if (models.Count > 0)
                {
                    if (models.Count == 1)
                        DuplicateObject(models[0]);
                    else
                        DuplicateMultipleObjects(models);
                }
            }
        }

        private void HandleCopyKey()
        {
            var selectedObjects = GetSelectedObjects();
            if (selectedObjects.Count > 0)
            {
                var models = selectedObjects.OfType<Models>().ToList();
                if (models.Count > 0)
                {
                    if (models.Count == 1)
                        CopyObject(models[0]);
                    else
                        CopyMultipleObjects(models);
                }
            }
        }

        private void HandlePasteKey()
        {
            PasteObjects();
        }

        private void HandleSelectAllKey()
        {
            SelectAllSelectableObjects();
        }
        #endregion

        #region Event Handlers
        private void OnBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            // Cancel the TreeView's built-in selection
            e.Cancel = true;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            TreeNode clickedNode = GetNodeAt(e.X, e.Y);
            
            if (clickedNode == null)
            {
                ClearAllSelections();
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            bool isCtrlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
            bool isShiftPressed = (ModifierKeys & Keys.Shift) == Keys.Shift;

            if (isCtrlPressed)
            {
                ToggleNodeSelection(clickedNode);
            }
            else if (isShiftPressed && m_lastSelectedNode != null)
            {
                SelectRange(m_lastSelectedNode, clickedNode);
            }
            else
            {
                SelectSingleNode(clickedNode);
            }

            UpdateLevelSelection();
            NotifySelectionChanged();
        }

        private void OnAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            if (m_nodeObjectMap.TryGetValue(e.Node, out object editedObject))
            {
                if (editedObject is Models model)
                {
                    model.Name = e.Label;
                    OnObjectRenamed?.Invoke(model);
                }
                else if (editedObject is Group group)
                {
                    group.Name = e.Label;
                    OnObjectRenamed?.Invoke(group);
                }
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode clickedNode = GetNodeAt(e.X, e.Y);
                if (clickedNode != null)
                {
                    // If right-click on non-selected node - select only that node
                    if (!m_selectedNodes.Contains(clickedNode))
                    {
                        SelectSingleNode(clickedNode);
                        UpdateLevelSelection();
                        NotifySelectionChanged();
                    }
                    
                    ShowContextMenu(clickedNode, e.Location);
                }
            }
        }
        #endregion

        #region Selection Management
        private void ClearAllSelections()
        {
            foreach (var node in m_selectedNodes)
            {
                node.BackColor = SystemColors.Window;
                node.ForeColor = SystemColors.WindowText;
            }
            m_selectedNodes.Clear();
            m_lastSelectedNode = null;
            SelectedNode = null;
            
            UpdateLevelSelection();
            NotifySelectionChanged();
        }

        private void SelectSingleNode(TreeNode node)
        {
            // Clear previous selections
            foreach (var selectedNode in m_selectedNodes)
            {
                selectedNode.BackColor = SystemColors.Window;
                selectedNode.ForeColor = SystemColors.WindowText;
            }
            m_selectedNodes.Clear();

            // Select the new node if it has an associated object
            if (m_nodeObjectMap.ContainsKey(node))
            {
                m_selectedNodes.Add(node);
                node.BackColor = SystemColors.Highlight;
                node.ForeColor = SystemColors.HighlightText;
                m_lastSelectedNode = node;
                SelectedNode = node;
            }
        }

        private void ToggleNodeSelection(TreeNode node)
        {
            if (!m_nodeObjectMap.ContainsKey(node)) return;

            if (m_selectedNodes.Contains(node))
            {
                m_selectedNodes.Remove(node);
                node.BackColor = SystemColors.Window;
                node.ForeColor = SystemColors.WindowText;
            }
            else
            {
                m_selectedNodes.Add(node);
                node.BackColor = SystemColors.Highlight;
                node.ForeColor = SystemColors.HighlightText;
                m_lastSelectedNode = node;
            }
        }

        private void SelectRange(TreeNode startNode, TreeNode endNode)
        {
            // Clear current selections
            foreach (var selectedNode in m_selectedNodes)
            {
                selectedNode.BackColor = SystemColors.Window;
                selectedNode.ForeColor = SystemColors.WindowText;
            }
            m_selectedNodes.Clear();

            // Get all nodes in tree order
            var allNodes = GetAllTreeNodes().ToList();
            int startIndex = allNodes.IndexOf(startNode);
            int endIndex = allNodes.IndexOf(endNode);

            if (startIndex == -1 || endIndex == -1) return;

            int minIndex = Math.Min(startIndex, endIndex);
            int maxIndex = Math.Max(startIndex, endIndex);

            for (int i = minIndex; i <= maxIndex; i++)
            {
                var node = allNodes[i];
                if (m_nodeObjectMap.ContainsKey(node))
                {
                    m_selectedNodes.Add(node);
                    node.BackColor = SystemColors.Highlight;
                    node.ForeColor = SystemColors.HighlightText;
                }
            }

            m_lastSelectedNode = endNode;
        }

        private void SelectAllSelectableObjects()
        {
            ClearAllSelections();
            
            foreach (var kvp in m_nodeObjectMap)
            {
                if (kvp.Value is ISelectable)
                {
                    m_selectedNodes.Add(kvp.Key);
                    kvp.Key.BackColor = SystemColors.Highlight;
                    kvp.Key.ForeColor = SystemColors.HighlightText;
                }
            }
            
            if (m_selectedNodes.Count > 0)
            {
                m_lastSelectedNode = m_selectedNodes.First();
            }
            
            UpdateLevelSelection();
            NotifySelectionChanged();
        }

        private void UpdateLevelSelection()
        {
            if (m_currentLevel == null) return;

            m_currentLevel.ClearSelectedModels();

            foreach (var node in m_selectedNodes)
            {
                if (m_nodeObjectMap.TryGetValue(node, out object selectedObject))
                {
                    if (selectedObject is ISelectable selectable)
                    {
                        selectable.Selected = true;
                    }
                }
            }
        }

        private void NotifySelectionChanged()
        {
            var selectedObjects = m_selectedNodes
                .Where(node => m_nodeObjectMap.ContainsKey(node))
                .Select(node => m_nodeObjectMap[node])
                .ToList();

            if (selectedObjects.Count == 1)
            {
                OnObjectSelected?.Invoke(selectedObjects[0]);
            }
            else if (selectedObjects.Count > 1)
            {
                OnMultipleObjectsSelected?.Invoke(selectedObjects);
            }
            else
            {
                OnObjectSelected?.Invoke(null);
            }
        }

        private List<object> GetSelectedObjects()
        {
            return m_selectedNodes
                .Where(node => m_nodeObjectMap.ContainsKey(node))
                .Select(node => m_nodeObjectMap[node])
                .ToList();
        }
        #endregion

        #region Context Menu
        private void ShowContextMenu(TreeNode node, Point location)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            var selectedObjects = GetSelectedObjects();

            if (selectedObjects.Count > 1)
            {
                // Multi-selection context menu
                var modelObjects = selectedObjects.OfType<Models>().ToList();
                if (modelObjects.Count > 0)
                {
                    bool anyInGroup = modelObjects.Any(model => m_currentLevel.FindGroupContaining(model) != null);
                    
                    if (!anyInGroup)
                    {
                        contextMenu.Items.Add("Create Group", null, (s, e) => CreateGroupFromSelection(modelObjects));
                        contextMenu.Items.Add("-");
                    }
                    else
                    {
                        contextMenu.Items.Add("Ungroup Selected Models", null, (s, e) => UngroupSelectedModels(modelObjects));
                        contextMenu.Items.Add("-");
                    }
                    
                    contextMenu.Items.Add($"Delete {modelObjects.Count} Objects", null, (s, e) => DeleteMultipleObjects(modelObjects));
                    contextMenu.Items.Add($"Duplicate {modelObjects.Count} Objects", null, (s, e) => DuplicateMultipleObjects(modelObjects));
                    contextMenu.Items.Add("-");
                    contextMenu.Items.Add($"Copy {modelObjects.Count} Objects", null, (s, e) => CopyMultipleObjects(modelObjects));
                    
                    if (m_clipboard.Count > 0)
                    {
                        contextMenu.Items.Add($"Paste {m_clipboard.Count} Objects", null, (s, e) => PasteObjects());
                    }
                }
            }
            else if (selectedObjects.Count == 1)
            {
                // Single selection context menu
                var selectedObject = selectedObjects[0];
                if (selectedObject is Models model)
                {
                    contextMenu.Items.Add("Rename", null, (s, e) => node.BeginEdit());
                    
                    // Check if this model is in a group
                    var parentGroup = m_currentLevel.FindGroupContaining(model);
                    if (parentGroup != null)
                    {
                        contextMenu.Items.Add("Ungroup from Group", null, (s, e) => UngroupSingleModel(model));
                        contextMenu.Items.Add("-");
                    }
                    
                    contextMenu.Items.Add("Delete", null, (s, e) => DeleteObject(selectedObject));
                    contextMenu.Items.Add("-");
                    contextMenu.Items.Add("Duplicate", null, (s, e) => DuplicateObject(selectedObject));
                    contextMenu.Items.Add("Copy", null, (s, e) => CopyObject(selectedObject));
                    
                    if (m_clipboard.Count > 0)
                    {
                        contextMenu.Items.Add($"Paste {m_clipboard.Count} Objects", null, (s, e) => PasteObjects());
                    }
                }
                else if (selectedObject is Group group)
                {
                    contextMenu.Items.Add("Rename", null, (s, e) => node.BeginEdit());
                    contextMenu.Items.Add("Ungroup", null, (s, e) => UngroupObject(group));
                    contextMenu.Items.Add("Delete", null, (s, e) => DeleteObject(selectedObject));
                }
            }
            else
            {
                // No selection - show paste if available
                if (m_clipboard.Count > 0)
                {
                    contextMenu.Items.Add($"Paste {m_clipboard.Count} Objects", null, (s, e) => PasteObjects());
                }
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(this, location);
            }
        }
        #endregion

        #region Group Management
        private void CreateGroupFromSelection(List<Models> models)
        {
            if (models.Count < 2)
            {
                MessageBox.Show("Please select at least 2 objects to create a group.", "Create Group", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create a new group
            var group = new Group(models, "New Group");
            
            // Add group to level
            m_currentLevel.AddGroup(group);
            
            // Remove individual models from level
            var modelsList = m_currentLevel.GetModelsList();
            foreach (var model in models)
            {
                modelsList.Remove(model);
            }
            
            ClearAllSelections();
            RefreshTree();
            
            // Select the new group
            if (m_objectNodeMap.TryGetValue(group, out TreeNode groupNode))
            {
                SelectSingleNode(groupNode);
                UpdateLevelSelection();
                NotifySelectionChanged();
                
                // Start renaming the group
                groupNode.BeginEdit();
            }
        }

        private void UngroupObject(Group group)
        {
            if (group == null) return;

            // Get the models from the group
            var models = group.Ungroup();
            
            // Remove group from level
            m_currentLevel.RemoveGroup(group);
            
            // Add individual models back to level
            foreach (var model in models)
            {
                m_currentLevel.AddModel(model);
            }
            
            ClearAllSelections();
            RefreshTree();
        }

        private void UngroupSingleModel(Models model)
        {
            var parentGroup = m_currentLevel.FindGroupContaining(model);
            if (parentGroup == null) return;

            // Remove the model from the group
            parentGroup.RemoveModels(new List<Models> { model });
            
            // Add the model back to the main models list
            m_currentLevel.AddModel(model);
            
            // If the group is now empty, remove it
            if (parentGroup.GroupModels.Count == 0)
            {
                m_currentLevel.RemoveGroup(parentGroup);
            }
            
            // Clear selection and refresh tree
            ClearAllSelections();
            RefreshTree();
        }

        private void UngroupSelectedModels(List<Models> models)
        {
            var groupsToCheck = new HashSet<Group>();
            
            foreach (var model in models)
            {
                var parentGroup = m_currentLevel.FindGroupContaining(model);
                if (parentGroup != null)
                {
                    // Remove the model from the group
                    parentGroup.RemoveModels(new List<Models> { model });
                    
                    // Add the model back to the main models list
                    m_currentLevel.AddModel(model);
                    
                    // Track groups that might become empty
                    groupsToCheck.Add(parentGroup);
                }
            }
            
            // Remove any groups that became empty
            foreach (var group in groupsToCheck)
            {
                if (group.GroupModels.Count == 0)
                {
                    m_currentLevel.RemoveGroup(group);
                }
            }
            
            // Clear selection and refresh tree
            ClearAllSelections();
            RefreshTree();
        }
        #endregion

        #region Object Operations
        private void DeleteObject(object obj)
        {
            if (obj is Models model)
            {
                var models = m_currentLevel.GetModelsList();
                models.Remove(model);
                RefreshTree();
            }
            else if (obj is Group group)
            {
                // Ungroup first, then delete all models
                var ungroupedModels = group.Ungroup();
                m_currentLevel.RemoveGroup(group);
                RefreshTree();
            }
        }

        private void DeleteMultipleObjects(List<Models> models)
        {
            var modelsList = m_currentLevel.GetModelsList();
            
            foreach (var model in models)
            {
                modelsList.Remove(model);
            }
            
            ClearAllSelections();
            RefreshTree();
        }

        private void DuplicateObject(object obj)
        {
            if (obj is Models model && m_game != null)
            {
                string modelName = model.Mesh?.Tag?.ToString() ?? "DefaultModel";
                string textureName = model.Material?.Diffuse?.Tag?.ToString() ?? "DefaultTexture";
                string effectName = model.Material?.Effect?.Tag?.ToString() ?? "DefaultEffect";
                
                var duplicate = new Models(m_game, modelName, textureName, effectName,
                    model.Position + new Microsoft.Xna.Framework.Vector3(0, 0, 0), model.Scale);
                
                duplicate.Rotation = model.Rotation;
                duplicate.Name = model.Name + "_Copy";
                
                m_currentLevel.AddModel(duplicate);
                RefreshTree();
            }
        }

        private void DuplicateMultipleObjects(List<Models> models)
        {
            foreach (var model in models)
            {
                DuplicateObject(model);
            }
        }

        private void CopyObject(object obj)
        {
            if (obj is Models model)
            {
                m_clipboard.Clear();
                m_clipboard.Add(model);
            }
        }

        private void CopyMultipleObjects(List<Models> models)
        {
            m_clipboard.Clear();
            m_clipboard.AddRange(models);
        }

        private void PasteObjects()
        {
            if (m_clipboard.Count == 0 || m_game == null) return;

            foreach (var model in m_clipboard)
            {
                string modelName = model.Mesh?.Tag?.ToString() ?? "DefaultModel";
                string textureName = model.Material?.Diffuse?.Tag?.ToString() ?? "DefaultTexture";
                string effectName = model.Material?.Effect?.Tag?.ToString() ?? "DefaultEffect";
                
                var copy = new Models(m_game, modelName, textureName, effectName,
                    model.Position + new Microsoft.Xna.Framework.Vector3(0, 0, 0), model.Scale);
                
                copy.Rotation = model.Rotation;
                copy.Name = model.Name + "_Copy";
                
                m_currentLevel.AddModel(copy);
            }
            
            RefreshTree();
        }
        #endregion

        #region Drag and Drop
        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = GetNodeAt(targetPoint);
            
            if (targetNode != null && e.Data.GetDataPresent(typeof(TreeNode)))
            {
                TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                if (draggedNode != targetNode && !IsChildNode(draggedNode, targetNode))
                {
                    HandleReparenting(draggedNode, targetNode);
                }
            }
        }

        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode node && m_nodeObjectMap.ContainsKey(node))
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void HandleReparenting(TreeNode draggedNode, TreeNode targetNode)
        {
            draggedNode.Remove();
            targetNode.Nodes.Add(draggedNode);
            targetNode.Expand();
        }

        private bool IsChildNode(TreeNode parent, TreeNode child)
        {
            TreeNode current = child.Parent;
            while (current != null)
            {
                if (current == parent) return true;
                current = current.Parent;
            }
            return false;
        }
        #endregion

        #region Utility Methods
        private IEnumerable<TreeNode> GetAllTreeNodes()
        {
            foreach (TreeNode rootNode in Nodes)
            {
                yield return rootNode;
                foreach (TreeNode childNode in GetChildNodes(rootNode))
                {
                    yield return childNode;
                }
            }
        }

        private IEnumerable<TreeNode> GetChildNodes(TreeNode parent)
        {
            foreach (TreeNode childNode in parent.Nodes)
            {
                yield return childNode;
                foreach (TreeNode grandChild in GetChildNodes(childNode))
                {
                    yield return grandChild;
                }
            }
        }
        #endregion

        #region Public Methods
        public void SelectObject(object obj)
        {
            if (m_objectNodeMap.TryGetValue(obj, out TreeNode node))
            {
                SelectedNode = node;
                node.EnsureVisible();
            }
        }

        public void UpdateObjectName(object obj, string newName)
        {
            if (m_objectNodeMap.TryGetValue(obj, out TreeNode node))
            {
                node.Text = newName;
            }
        }

        public void SyncSelectionFromLevel()
        {
            if (m_currentLevel == null) return;

            var selectedModels = m_currentLevel.GetSelectedModels();
            if (selectedModels.Count > 0)
            {
                SelectObject(selectedModels[0]);
            }
        }
        #endregion
    }
}