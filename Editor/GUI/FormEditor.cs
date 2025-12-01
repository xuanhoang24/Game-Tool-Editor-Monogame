using Editor.Editor;
using Editor.Engine;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Editor
{
    public partial class FormEditor : Form
    {
        public GameEditor Game { get; set; }
        private PictureBox minimapPictureBox;
        private MinimapRenderer minimapRenderer;
        private Timer minimapTimer;

        public FormEditor()
        {
            InitializeComponent();
            InitializeMinimap();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Game.Exit();
        }

        private void FormEditor_Load(object sender, EventArgs e)
        {
            Text = "Our Cool Editor";
        }

        private void splitContainer_Panel1_SizeChanged(object sender, EventArgs e)
        {
            if (Game == null) return;
            Game.AdjustAspectRatio();
        }

        private void FormEditor_SizeChanged(object sender, EventArgs e)
        {
            if (Game == null) return;
            Game.AdjustAspectRatio();
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Game.Project = new(Game.Content, sfd.FileName);
                UpdateTitle();
                Game.AdjustAspectRatio();
            }
            saveToolStripMenuItem_Click(sender, e);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Game.Project == null) return;

            // If no folder set, prompt for save location
            if (string.IsNullOrEmpty(Game.Project.Folder))
            {
                SaveFileDialog sfd = new();
                sfd.Filter = "OCE Files|*.oce";
                sfd.FileName = Game.Project.Name;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Game.Project.Folder = Path.GetDirectoryName(sfd.FileName);
                    Game.Project.Name = Path.GetFileName(sfd.FileName);
                    UpdateTitle();
                }
                else
                {
                    return;
                }
            }

            string fname = Path.Combine(Game.Project.Folder, Game.Project.Name);
            using var stream = File.Open(fname, FileMode.Create);
            using var writer = new BinaryWriter(stream, Encoding.UTF8, false);
            Game.Project.Serialize(writer);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "OCE Files|*.oce";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using var stream = File.Open(ofd.FileName, FileMode.Open);
                using var reader = new BinaryReader(stream, Encoding.UTF8, false);
                Game.Project = new();
                Game.Project.Deserialize(reader, Game.Content);
                UpdateTitle();
                Game.AdjustAspectRatio();
            }
        }

        private void addSunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureProject();
            Game.Project.CurrentLevel.AddSun(Game.Content);
        }

        private void addPlanetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureProject();
            Game.Project.CurrentLevel.AddPlanet(Game.Content);
        }

        private void addMoonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureProject();
            Game.Project.CurrentLevel.AddMoon(Game.Content);
        }

        private void EnsureProject()
        {
            if (Game.Project == null)
            {
                Game.Project = new(Game.Content, "Untitled.oce");
                Game.AdjustAspectRatio();
                UpdateTitle();
            }
        }

        private void UpdateTitle()
        {
            Text = Game.Project != null ? "Our Cool Editor - " + Game.Project.Name : "Our Cool Editor";
        }

        private void InitializeMinimap()
        {
            minimapPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(25, 30, 40),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            splitContainer.Panel2.Controls.Add(minimapPictureBox);

            minimapTimer = new Timer { Interval = 33 };
            minimapTimer.Tick += MinimapTimer_Tick;
            minimapTimer.Start();
        }

        private void MinimapTimer_Tick(object sender, EventArgs e)
        {
            if (Game?.Project?.CurrentLevel == null) return;

            int width = Math.Max(minimapPictureBox.Width, 100);
            int height = Math.Max(minimapPictureBox.Height, 100);

            if (minimapRenderer == null || minimapRenderer.Width != width || minimapRenderer.Height != height)
            {
                minimapRenderer?.Dispose();
                minimapRenderer = new MinimapRenderer(width, height);
            }

            var world = Game.Project.CurrentLevel.GetWorld();
            if (world != null)
            {
                minimapPictureBox.Image = minimapRenderer.Render(world);
            }
        }
    }
}
