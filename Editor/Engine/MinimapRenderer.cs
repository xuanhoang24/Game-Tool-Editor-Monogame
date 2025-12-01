using Editor.Engine.ECS;
using Editor.Engine.ECS.Components;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Editor.Engine
{
    public class MinimapRenderer
    {
        private Bitmap m_bitmap;
        private Graphics m_graphics;
        private float m_scale = 1.5f;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public MinimapRenderer(int width, int height)
        {
            Width = width;
            Height = height;
            m_bitmap = new Bitmap(width, height);
            m_graphics = Graphics.FromImage(m_bitmap);
            m_graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        public Bitmap Render(World world)
        {
            m_graphics.Clear(Color.FromArgb(25, 30, 40)); // Dark blue background
            DrawGrid();
            DrawEntities(world);
            return m_bitmap;
        }

        private void DrawGrid()
        {
            int cx = Width / 2, cy = Height / 2; // Center point
            using (Pen grid = new Pen(Color.FromArgb(45, 50, 60)))
            using (Pen cross = new Pen(Color.FromArgb(80, 90, 100), 2))
            {
                // Draw vertical and horizontal grid lines every 20 pixels
                for (int x = cx % 20; x < Width; x += 20) m_graphics.DrawLine(grid, x, 0, x, Height);
                for (int y = cy % 20; y < Height; y += 20) m_graphics.DrawLine(grid, 0, y, Width, y);
                
                // Draw center crosshair
                m_graphics.DrawLine(cross, cx - 10, cy, cx + 10, cy);
                m_graphics.DrawLine(cross, cx, cy - 10, cx, cy + 10);
            }
        }

        private void DrawEntities(World world)
        {
            foreach (var e in world.GetEntities())
            {
                if (!e.HasComponent<TransformComponent>()) continue;

                var t = e.GetComponent<TransformComponent>();
                // Convert 3D position (X,Z) to 2D minimap coordinates (top-down view)
                float x = Width / 2 + t.Position.X / m_scale;
                float y = Height / 2 + t.Position.Z / m_scale;

                var (color, size) = GetEntityStyle(e);
                using (var brush = new SolidBrush(color))
                    m_graphics.FillEllipse(brush, x - size / 2, y - size / 2, size, size);

                DrawOrbit(world, e, color);
            }
        }

        private (Color color, float size) GetEntityStyle(Entity e)
        {
            Color color = Color.White;
            float size = 4f;

            // Get color from texture center pixel
            if (e.HasComponent<MeshComponent>())
                color = SampleTexture(e.GetComponent<MeshComponent>().Texture);

            // Set size based on entity type
            if (e.HasComponent<TagComponent>())
                size = e.GetComponent<TagComponent>().Tag switch { "Sun" => 12f, "World" => 6f, "Moon" => 3f, _ => 4f };

            return (color, size);
        }

        private void DrawOrbit(World world, Entity e, Color color)
        {
            if (!e.HasComponent<OrbitComponent>()) return;

            var orbit = e.GetComponent<OrbitComponent>();
            var parent = world.GetEntity(orbit.ParentEntityId); // Find parent (sun or planet)
            
            if (parent?.HasComponent<TransformComponent>() == true)
            {
                var pt = parent.GetComponent<TransformComponent>();
                // Draw circle around parent showing orbit path
                float px = Width / 2 + pt.Position.X / m_scale;
                float py = Height / 2 + pt.Position.Z / m_scale;
                float r = orbit.OrbitRadius / m_scale;

                using (var pen = new Pen(Color.FromArgb(40, color))) // Faint orbit line
                    m_graphics.DrawEllipse(pen, px - r, py - r, r * 2, r * 2);
            }
        }

        private Color SampleTexture(Texture tex)
        {
            if (tex is Texture2D tex2D)
            {
                try
                {
                    // Read center pixel color from texture
                    var data = new Microsoft.Xna.Framework.Color[1];
                    tex2D.GetData(0, new Microsoft.Xna.Framework.Rectangle(tex2D.Width / 2, tex2D.Height / 2, 1, 1), data, 0, 1);
                    return Color.FromArgb(data[0].A, data[0].R, data[0].G, data[0].B);
                }
                catch { }
            }
            return Color.White;
        }

        public void Dispose()
        {
            m_graphics?.Dispose();
            m_bitmap?.Dispose();
        }
    }
}
