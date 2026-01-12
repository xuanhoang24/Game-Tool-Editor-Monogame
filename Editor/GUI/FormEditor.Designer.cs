namespace Editor
{
    partial class FormEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            assetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            createPrefabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            scriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openScriptEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            splitContainer = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            splitContainer3 = new System.Windows.Forms.SplitContainer();
            sceneTreeView = new SceneTreeView();
            listBoxPrefabs = new System.Windows.Forms.ListBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            propertyGrid = new System.Windows.Forms.PropertyGrid();
            listBoxAssets = new System.Windows.Forms.ListBox();
            labelSceneHierarchy = new System.Windows.Forms.Label();
            labelPrefabs = new System.Windows.Forms.Label();
            labelProperties = new System.Windows.Forms.Label();
            labelAssets = new System.Windows.Forms.Label();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, assetsToolStripMenuItem, scriptsToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(1379, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { projectToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // projectToolStripMenuItem
            // 
            projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { createToolStripMenuItem, saveToolStripMenuItem, loadToolStripMenuItem });
            projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            projectToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            projectToolStripMenuItem.Text = "Project";
            // 
            // createToolStripMenuItem
            // 
            createToolStripMenuItem.Name = "createToolStripMenuItem";
            createToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            createToolStripMenuItem.Text = "Create";
            createToolStripMenuItem.Click += createToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            loadToolStripMenuItem.Text = "Load";
            loadToolStripMenuItem.Click += loadToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // assetsToolStripMenuItem
            // 
            assetsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { importToolStripMenuItem, createPrefabToolStripMenuItem });
            assetsToolStripMenuItem.Name = "assetsToolStripMenuItem";
            assetsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            assetsToolStripMenuItem.Text = "Assets";
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            importToolStripMenuItem.Text = "Import";
            importToolStripMenuItem.Click += importToolStripMenuItem_Click;
            // 
            // createPrefabToolStripMenuItem
            // 
            createPrefabToolStripMenuItem.Name = "createPrefabToolStripMenuItem";
            createPrefabToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            createPrefabToolStripMenuItem.Text = "Create Prefab";
            createPrefabToolStripMenuItem.Click += createPrefabToolStripMenuItem_Click;
            // 
            // scriptsToolStripMenuItem
            // 
            scriptsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { openScriptEditorToolStripMenuItem });
            scriptsToolStripMenuItem.Name = "scriptsToolStripMenuItem";
            scriptsToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            scriptsToolStripMenuItem.Text = "Scripts";
            // 
            // openScriptEditorToolStripMenuItem
            // 
            openScriptEditorToolStripMenuItem.Name = "openScriptEditorToolStripMenuItem";
            openScriptEditorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            openScriptEditorToolStripMenuItem.Text = "Open Script Editor";
            openScriptEditorToolStripMenuItem.Click += openScriptEditorToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 850);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(1379, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new System.Drawing.Size(54, 17);
            toolStripStatusLabel.Text = "ToolStrip";
            // 
            // splitContainer
            // 
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Location = new System.Drawing.Point(0, 24);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(splitContainer2);
            splitContainer.Panel1.SizeChanged += splitContainer_Panel1_SizeChanged;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(splitContainer1);
            splitContainer.Size = new System.Drawing.Size(1379, 826);
            splitContainer.SplitterDistance = 967;
            splitContainer.TabIndex = 2;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(splitContainer3);
            splitContainer2.Size = new System.Drawing.Size(967, 826);
            splitContainer2.SplitterDistance = 320;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer3.Location = new System.Drawing.Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(sceneTreeView);
            splitContainer3.Panel1.Controls.Add(labelSceneHierarchy);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(listBoxPrefabs);
            splitContainer3.Panel2.Controls.Add(labelPrefabs);
            splitContainer3.Size = new System.Drawing.Size(320, 826);
            splitContainer3.SplitterDistance = 389;
            splitContainer3.TabIndex = 0;
            // 
            // sceneTreeView
            // 
            sceneTreeView.AllowDrop = true;
            sceneTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            sceneTreeView.HideSelection = false;
            sceneTreeView.LabelEdit = true;
            sceneTreeView.Location = new System.Drawing.Point(0, 20);
            sceneTreeView.Name = "sceneTreeView";
            sceneTreeView.Size = new System.Drawing.Size(320, 369);
            sceneTreeView.TabIndex = 0;
            // 
            // labelSceneHierarchy
            // 
            labelSceneHierarchy.BackColor = System.Drawing.SystemColors.Control;
            labelSceneHierarchy.Dock = System.Windows.Forms.DockStyle.Top;
            labelSceneHierarchy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            labelSceneHierarchy.Location = new System.Drawing.Point(0, 0);
            labelSceneHierarchy.Name = "labelSceneHierarchy";
            labelSceneHierarchy.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            labelSceneHierarchy.Size = new System.Drawing.Size(320, 20);
            labelSceneHierarchy.TabIndex = 1;
            labelSceneHierarchy.Text = "Scene Hierarchy";
            // 
            // listBoxPrefabs
            // 
            listBoxPrefabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            listBoxPrefabs.FormattingEnabled = true;
            listBoxPrefabs.ItemHeight = 15;
            listBoxPrefabs.Location = new System.Drawing.Point(0, 20);
            listBoxPrefabs.Name = "listBoxPrefabs";
            listBoxPrefabs.Size = new System.Drawing.Size(320, 413);
            listBoxPrefabs.TabIndex = 0;
            // 
            // labelPrefabs
            // 
            labelPrefabs.BackColor = System.Drawing.SystemColors.Control;
            labelPrefabs.Dock = System.Windows.Forms.DockStyle.Top;
            labelPrefabs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            labelPrefabs.Location = new System.Drawing.Point(0, 0);
            labelPrefabs.Name = "labelPrefabs";
            labelPrefabs.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            labelPrefabs.Size = new System.Drawing.Size(320, 20);
            labelPrefabs.TabIndex = 1;
            labelPrefabs.Text = "Prefabs";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(propertyGrid);
            splitContainer1.Panel1.Controls.Add(labelProperties);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(listBoxAssets);
            splitContainer1.Panel2.Controls.Add(labelAssets);
            splitContainer1.Size = new System.Drawing.Size(408, 826);
            splitContainer1.SplitterDistance = 415;
            splitContainer1.TabIndex = 1;
            // 
            // propertyGrid
            // 
            propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            propertyGrid.Location = new System.Drawing.Point(0, 20);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new System.Drawing.Size(408, 395);
            propertyGrid.TabIndex = 0;
            // 
            // labelProperties
            // 
            labelProperties.BackColor = System.Drawing.SystemColors.Control;
            labelProperties.Dock = System.Windows.Forms.DockStyle.Top;
            labelProperties.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            labelProperties.Location = new System.Drawing.Point(0, 0);
            labelProperties.Name = "labelProperties";
            labelProperties.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            labelProperties.Size = new System.Drawing.Size(408, 20);
            labelProperties.TabIndex = 1;
            labelProperties.Text = "Properties";
            // 
            // listBoxAssets
            // 
            listBoxAssets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            listBoxAssets.FormattingEnabled = true;
            listBoxAssets.ItemHeight = 15;
            listBoxAssets.Location = new System.Drawing.Point(0, 20);
            listBoxAssets.Name = "listBoxAssets";
            listBoxAssets.Size = new System.Drawing.Size(408, 387);
            listBoxAssets.TabIndex = 0;
            // 
            // labelAssets
            // 
            labelAssets.BackColor = System.Drawing.SystemColors.Control;
            labelAssets.Dock = System.Windows.Forms.DockStyle.Top;
            labelAssets.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            labelAssets.Location = new System.Drawing.Point(0, 0);
            labelAssets.Name = "labelAssets";
            labelAssets.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            labelAssets.Size = new System.Drawing.Size(408, 20);
            labelAssets.TabIndex = 1;
            labelAssets.Text = "Assets";
            // 
            // FormEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1379, 872);
            Controls.Add(splitContainer);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "FormEditor";
            Text = "Our Cool Editor";
            SizeChanged += FormEditor_SizeChanged;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        public System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBoxAssets;
        private System.Windows.Forms.ToolStripMenuItem assetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        public System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        public SceneTreeView sceneTreeView;
        private System.Windows.Forms.ListBox listBoxPrefabs;
        private System.Windows.Forms.ToolStripMenuItem createPrefabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openScriptEditorToolStripMenuItem;
        private System.Windows.Forms.Label labelSceneHierarchy;
        private System.Windows.Forms.Label labelPrefabs;
        private System.Windows.Forms.Label labelProperties;
        private System.Windows.Forms.Label labelAssets;
    }
}