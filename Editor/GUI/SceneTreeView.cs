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
        private GameEditor m_game;
        private Level m_currentLevel;
        private Dictionary<TreeNode, object> m_nodeObjectMap = new();
        private Dictionary<object, TreeNode> m_objectNodeMap = new();

        public event Action<object> OnObjectSelected;
        public event Action<object> OnObjectRenamed;

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
            
            // Set up event handlers
            AfterSelect += OnAfterSelect;
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

        public void RefreshTree()
        {
            if (m_game?.Project?.CurrentLevel == null) return;

            m_currentLevel = m_game.Project.CurrentLevel;
            
            BeginUpdate();
            Nodes.Clear();
            m_nodeObjectMap.Clear();
            m_objectNodeMap.Clear();

            // Create root level node
            TreeNode levelNode = new TreeNode("Level")
            {
                Tag = "Level"
            };
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

            // Add models container
            TreeNode modelsNode = new TreeNode("Models")
            {
                Tag = "ModelsContainer"
            };
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
            modelsNode.Expand();
            EndUpdate();
        }

        private void OnAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_nodeObjectMap.TryGetValue(e.Node, out object selectedObject))
            {
                // Clear previous selections
                m_currentLevel?.ClearSelectedModels();
                
                // Set new selection
                if (selectedObject is ISelectable selectable)
                {
                    selectable.Selected = true;
                }
                
                OnObjectSelected?.Invoke(selectedObject);
            }
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
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode clickedNode = GetNodeAt(e.X, e.Y);
                if (clickedNode != null)
                {
                    SelectedNode = clickedNode;
                    ShowContextMenu(clickedNode, e.Location);
                }
            }
        }

        private void ShowContextMenu(TreeNode node, Point location)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            if (m_nodeObjectMap.TryGetValue(node, out object nodeObject))
            {
                if (nodeObject is Models)
                {
                    contextMenu.Items.Add("Rename", null, (s, e) => node.BeginEdit());
                    contextMenu.Items.Add("Delete", null, (s, e) => DeleteObject(nodeObject));
                    contextMenu.Items.Add("-");
                    contextMenu.Items.Add("Duplicate", null, (s, e) => DuplicateObject(nodeObject));
                }
            }

            if (node.Tag?.ToString() == "ModelsContainer")
            {
                contextMenu.Items.Add("Add Empty Model", null, (s, e) => AddEmptyModel());
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(this, location);
            }
        }

        private void DeleteObject(object obj)
        {
            if (obj is Models model)
            {
                var models = m_currentLevel.GetModelsList();
                models.Remove(model);
                RefreshTree();
            }
        }

        private void DuplicateObject(object obj)
        {
            if (obj is Models model && m_game != null)
            {
                // Get the original model's asset names from tags
                string modelName = model.Mesh?.Tag?.ToString() ?? "DefaultModel";
                string textureName = model.Material?.Diffuse?.Tag?.ToString() ?? "DefaultTexture";
                string effectName = model.Material?.Effect?.Tag?.ToString() ?? "DefaultEffect";
                
                // Create a duplicate using the proper constructor
                var duplicate = new Models(m_game, modelName, textureName, effectName,
                    model.Position + new Microsoft.Xna.Framework.Vector3(10, 0, 0), model.Scale);
                
                duplicate.Rotation = model.Rotation;
                duplicate.Name = model.Name + "_Copy";
                
                m_currentLevel.AddModel(duplicate);
                RefreshTree();
            }
        }

        private void AddEmptyModel()
        {
            if (m_game != null)
            {
                // Create an empty model using default assets
                var emptyModel = new Models(m_game, "DefaultModel", "DefaultTexture", "DefaultEffect",
                    Microsoft.Xna.Framework.Vector3.Zero, 1.0f);
                emptyModel.Name = "Empty Model";
                
                m_currentLevel.AddModel(emptyModel);
                RefreshTree();
            }
        }

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
    }
}