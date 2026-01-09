using Editor.Editor;
using System;
using System.IO;
using System.Windows.Forms;

namespace Editor.GUI
{
    /// <summary>
    /// Script viewer form
    /// </summary>
    public partial class ScriptViewer : Form
    {
        private ComboBox scriptSelector;
        private RichTextBox scriptDisplay;
        private Project currentProject;

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
            Text = "LUA Script Viewer";
            Size = new System.Drawing.Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;

            // Script selector
            scriptSelector = new ComboBox();
            scriptSelector.Dock = DockStyle.Top;
            scriptSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            scriptSelector.SelectedIndexChanged += ScriptSelector_SelectedIndexChanged;

            // Script display
            scriptDisplay = new RichTextBox();
            scriptDisplay.Dock = DockStyle.Fill;
            scriptDisplay.ReadOnly = true;
            LUAHighlighter.ConfigureForLua(scriptDisplay);

            Controls.Add(scriptDisplay);
            Controls.Add(scriptSelector);
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
                return;

            try
            {
                var luaFiles = Directory.GetFiles(currentProject.ScriptFolder, "*.lua");
                foreach (var file in luaFiles)
                {
                    scriptSelector.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scripts: {ex.Message}", "Error");
            }
        }

        private void ScriptSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scriptSelector.SelectedItem == null) return;

            string scriptPath = Path.Combine(currentProject.ScriptFolder, scriptSelector.SelectedItem.ToString());
            
            try
            {
                if (File.Exists(scriptPath))
                {
                    scriptDisplay.Text = File.ReadAllText(scriptPath);
                    LUAHighlighter.ApplyHighlighting(scriptDisplay);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading script: {ex.Message}", "Error");
            }
        }
    }
}