using Editor.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;

namespace Editor.Editor
{
    public class GameEditor : Game
    {
        internal Project Project { get; set; }
        private GraphicsDeviceManager m_graphics;
        private FormEditor m_parent;
        public GameEditor()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public GameEditor(FormEditor _parent) : this()
        {
            m_parent = _parent;
            Form gameForm = Control.FromHandle(Window.Handle) as Form;
            gameForm.TopLevel = false;
            gameForm.Dock = DockStyle.Fill;
            gameForm.FormBorderStyle = FormBorderStyle.None;
            m_parent.splitContainer.Panel1.Controls.Add(gameForm);
        }
        protected override void Initialize()
        {
            RasterizerState state = new RasterizerState();
            state.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = state;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Project = new(Content, "Untitled.oce");
            AdjustAspectRatio();
        }

        protected override void Update(GameTime gameTime)
        {
            Project?.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Project?.Render();

            base.Draw(gameTime);
        }

        public void AdjustAspectRatio()
        {
            if (Project == null) return;
            Camera c = Project.CurrentLevel.GetCamera();
            c.Update(c.Position, m_graphics.GraphicsDevice.Viewport.AspectRatio);
        }
    }
}
