using System.Drawing;
using System.Windows.Forms;

namespace PowerSwitcher.Rendering
{
    /// <summary>
    /// Custom color palette for context menu strip renderer
    /// </summary>
    public class ColorTable : ProfessionalColorTable
    {
        public ColorTable() : base()
        {

        }

        /// <summary>
        /// Returns menu item border color
        /// </summary>
        public override Color MenuItemBorder => Color.LightBlue;

        /// <summary>
        /// Returns menu item hover color
        /// </summary>
        public override Color MenuItemSelected => Color.LightBlue;

        public override Color ToolStripDropDownBackground => Color.White;

        public override Color ImageMarginGradientBegin => Color.White;

        public override Color ImageMarginGradientMiddle => Color.White;

        public override Color ImageMarginGradientEnd => Color.White;
    }
}
