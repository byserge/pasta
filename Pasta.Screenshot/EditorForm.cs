using Pasta.BasicEffects;
using Pasta.Core;
using Pasta.Screenshot.Effects;
using Pasta.Screenshot.ExportActions;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Pasta.Screenshot
{
    public partial class EditorForm : Form
    {
        private EffectsManager effectsManager;
        private ExportManager exportManager;

        public EditorForm()
        {
            InitializeComponent();
            effectsManager = new EffectsManager();
            exportManager = new ExportManager();
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {
            effectsManager.Invalidated += Effects_Invalidated;            
            PrintScreen();
        }

        private void EditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            effectsManager.Invalidated -= Effects_Invalidated;
        }

        private void PrintScreen()
        {
            var bounds = Screen.AllScreens
                .Select(screen => screen.Bounds)
                .Aggregate((acc, screenBounds) => Rectangle.Union(acc, screenBounds));
            this.Location = bounds.Location;
            this.Size = bounds.Size;

            effectsManager.AddEffect(new ScreenshotEffect());
            effectsManager.AddEffect(new SelectionEffect());
            Invalidate(false);
        }

        private async void EditorForm_DoubleClick(object sender, EventArgs e)
        {
            using (var image = effectsManager.CreateImage(this.Size))
            {
                await exportManager.ExportAsync(image);
            }

            Close();
        }

        private void EditorForm_Paint(object sender, PaintEventArgs e)
        {
            effectsManager.ApplyEffects(e.Graphics, this.Bounds, e.ClipRectangle);
        }

        private void EditorForm_MouseDown(object sender, MouseEventArgs e)
        {
            effectsManager.OnMouseDown(e);
        }

        private void EditorForm_MouseUp(object sender, MouseEventArgs e)
        {
            effectsManager.OnMouseUp(e);
        }

        private void EditorForm_MouseMove(object sender, MouseEventArgs e)
        {
            effectsManager.OnMouseMove(e);
        }

        private void Effects_Invalidated(object sender, InvalidatedEventArgs e)
        {
            this.Invalidate(e.Rectangle, false);
        }
    }
}
