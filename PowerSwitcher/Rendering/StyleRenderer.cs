using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PowerSwitcher.Rendering
{
    /// <summary>
    /// Custom Renderer for ContextMenuStrip component
    /// </summary>
    public class StyleRenderer : ToolStripProfessionalRenderer
    {
        private readonly Pen LineColor = Pens.Black;
        public StyleRenderer() : base(new ColorTable())
        {

        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(e.ArrowRectangle.Location, e.ArrowRectangle.Size);
            r.Inflate(-4, -6);

            e.Graphics.DrawLines(this.LineColor, new Point[]
            {
                new Point(r.Left, r.Top),
                new Point(r.Right, r.Top + r.Height / 2),
                new Point(r.Left, r.Top+ r.Height)
            });
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(e.ImageRectangle.Location, e.ImageRectangle.Size);
            r.Inflate(-4, -6);

            e.Graphics.DrawLines(this.LineColor, new Point[] 
            {
                new Point(r.Left, r.Bottom - r.Height / 2),
                new Point(r.Left + r.Width / 3,  r.Bottom),
                new Point(r.Right, r.Top)
            });
        }
    }
}
