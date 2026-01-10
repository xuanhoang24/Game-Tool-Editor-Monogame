using Editor.Editor;
using System;
using System.IO;
using System.Windows.Forms;

namespace Editor.GUI
{
    /// <summary>
    /// Script editor form
    /// </summary>
    public partial class ScriptViewer : Form
    {
        private ComboBox scriptSelector;
        private RichTextBox scriptDisplay;
        private Button saveButton;
        private Label statusLabel;
        private Project currentProject;
        private string currentScriptPath = string.Empty;
        private bool isModified = false;
        private Timer highlightTimer;
        private bool isApplyingHighlighting = false;

        public ScriptViewer()
        {
            InitializeComponent();
        }

        internal ScriptViewer(Project project) : this()
        {
            SetProject(project);
        }

        private void InitializeComponent()
        {
            Text = "Lua Script Editor";
            Size = new System.Drawing.Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;

            // Create toolbar panel
            var toolbarPanel = new Panel();
            toolbarPanel.Height = 35;
            toolbarPanel.Dock = DockStyle.Top;

            // Script selector
            scriptSelector = new ComboBox();
            scriptSelector.Location = new System.Drawing.Point(10, 6);
            scriptSelector.Width = 200;
            scriptSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            scriptSelector.SelectedIndexChanged += ScriptSelector_SelectedIndexChanged;

            // Save button
            saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Location = new System.Drawing.Point(220, 5);
            saveButton.Size = new System.Drawing.Size(60, 25);
            saveButton.Enabled = false;
            saveButton.Click += SaveButton_Click;

            // Add buttons to toolbar
            toolbarPanel.Controls.Add(scriptSelector);
            toolbarPanel.Controls.Add(saveButton);

            // Status label
            statusLabel = new Label();
            statusLabel.Text = "Ready - Select a script to edit";
            statusLabel.Height = 20;
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.BackColor = System.Drawing.SystemColors.Control;
            statusLabel.BorderStyle = BorderStyle.Fixed3D;

            // Script display
            scriptDisplay = new RichTextBox();
            scriptDisplay.Dock = DockStyle.Fill;
            scriptDisplay.ReadOnly = false;
            scriptDisplay.TextChanged += ScriptDisplay_TextChanged;
            scriptDisplay.KeyDown += ScriptDisplay_KeyDown;
            LUAHighlighter.ConfigureForLua(scriptDisplay);

            // Highlight timer
            highlightTimer = new Timer();
            highlightTimer.Interval = 300;
            highlightTimer.Tick += HighlightTimer_Tick;

            // Add controls to form
            Controls.Add(scriptDisplay);
            Controls.Add(toolbarPanel);
            Controls.Add(statusLabel);
        }

        internal void SetProject(Project project)
        {
            currentProject = project;
            LoadScriptList();
        }

        private void LoadScriptList()
        {
            scriptSelector.Items.Clear();

            if (currentProject == null || !Directory.Exists(currentProject.ScriptFolder))
            {
                statusLabel.Text = "No project loaded";
                return;
            }

            try
            {
                var luaFiles = Directory.GetFiles(currentProject.ScriptFolder, "*.lua");
                foreach (var file in luaFiles)
                {
                    scriptSelector.Items.Add(Path.GetFileName(file));
                }

                if (luaFiles.Length > 0)
                {
                    statusLabel.Text = $"Found {luaFiles.Length} Lua scripts - Select one to edit";
                }
                else
                {
                    statusLabel.Text = "No Lua scripts found in project";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error loading scripts: {ex.Message}";
                MessageBox.Show($"Error loading scripts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckUnsavedChanges()
        {
            if (!isModified) return true; // No changes, continue

            var result = MessageBox.Show(
                "You have unsaved changes. Do you want to save them first?",
                "Unsaved Changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
            {
                return false;
            }
            else if (result == DialogResult.Yes)
            {
                SaveCurrentScript();
            }
            
            return true;
        }

        private void ScriptSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scriptSelector.SelectedItem == null) return;

            if (!CheckUnsavedChanges()) return;

            LoadScript(scriptSelector.SelectedItem.ToString());
        }

        private void LoadScript(string scriptName)
        {
            if (currentProject == null) return;

            currentScriptPath = Path.Combine(currentProject.ScriptFolder, scriptName);
            
            try
            {
                if (File.Exists(currentScriptPath))
                {
                    scriptDisplay.Text = File.ReadAllText(currentScriptPath);
                    ApplySyntaxHighlighting();
                    
                    isModified = false;
                    UpdateUI();
                    statusLabel.Text = $"Loaded: {scriptName} - Ready to edit";
                }
                else
                {
                    statusLabel.Text = $"Script not found: {scriptName}";
                    MessageBox.Show($"Script file not found: {scriptName}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error loading script: {ex.Message}";
                MessageBox.Show($"Error loading script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ScriptDisplay_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentScriptPath) && !isApplyingHighlighting)
            {
                isModified = true;
                UpdateUI();
                
                // Restart the highlight timer
                highlightTimer.Stop();
                highlightTimer.Start();
            }
        }

        private void HighlightTimer_Tick(object sender, EventArgs e)
        {
            highlightTimer.Stop();
            ApplySyntaxHighlighting();
        }

        private void ApplySyntaxHighlighting()
        {
            if (isApplyingHighlighting) return;
            
            isApplyingHighlighting = true;
            try
            {
                LUAHighlighter.ApplyHighlighting(scriptDisplay);
            }
            finally
            {
                isApplyingHighlighting = false;
            }
        }

        private void ScriptDisplay_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+S to save
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveCurrentScript();
                e.Handled = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveCurrentScript();
        }

        private void SaveCurrentScript()
        {
            if (string.IsNullOrEmpty(currentScriptPath))
            {
                statusLabel.Text = "No script selected to save";
                return;
            }

            try
            {
                File.WriteAllText(currentScriptPath, scriptDisplay.Text);
                isModified = false;
                UpdateUI();
                statusLabel.Text = $"Saved: {Path.GetFileName(currentScriptPath)}";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error saving: {ex.Message}";
                MessageBox.Show($"Error saving script: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUI()
        {
            bool hasScript = !string.IsNullOrEmpty(currentScriptPath);
            
            saveButton.Enabled = hasScript && isModified;
            
            // Update window title to show modification status
            string title = "Lua Script Editor";
            if (hasScript)
            {
                string scriptName = Path.GetFileName(currentScriptPath);
                title += $" - {scriptName}";
                if (isModified)
                {
                    title += " *";
                }
            }
            Text = title;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop and dispose timer
            highlightTimer?.Stop();
            
            if (!CheckUnsavedChanges())
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                highlightTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}