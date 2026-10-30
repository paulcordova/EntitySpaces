using System.Drawing;
using System.Windows.Forms;

namespace EntitySpaces.Common
{
    public class AppThemeColorTable : ProfessionalColorTable
    {
        private readonly Color _bgColor;
        private readonly Color _selectionColor;

        public AppThemeColorTable(Color bgColor, Color selectionColor)
        {
            _bgColor = bgColor;
            _selectionColor = selectionColor;
        }

        public override Color ToolStripDropDownBackground => _bgColor;
        public override Color ImageMarginGradientBegin => _bgColor;
        public override Color ImageMarginGradientMiddle => _bgColor;
        public override Color ImageMarginGradientEnd => _bgColor;

        public override Color MenuItemSelected => _selectionColor;
        public override Color MenuItemBorder => Color.Transparent;
        public override Color MenuBorder => Color.FromArgb(80, 80, 80);

        public override Color ToolStripGradientBegin => _bgColor;
        public override Color ToolStripGradientMiddle => _bgColor;
        public override Color ToolStripGradientEnd => _bgColor;

        public override Color ButtonSelectedHighlight => _selectionColor;
        public override Color ButtonSelectedGradientBegin => _selectionColor;
        public override Color ButtonSelectedGradientEnd => _selectionColor;
        public override Color ButtonPressedGradientBegin => _selectionColor;
        public override Color ButtonPressedGradientEnd => _selectionColor;
    }

    public class KryptonMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly Color _bgColor;
        private readonly Color _selectionColor;
        private readonly Color _foreColor;

        public KryptonMenuRenderer(Color bgColor, Color selectionColor, Color foreColor)
            : base(new AppThemeColorTable(bgColor, selectionColor))
        {
            _bgColor = bgColor;
            _selectionColor = selectionColor;
            _foreColor = foreColor;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(_bgColor))
            {
                e.Graphics.FillRectangle(b, e.ToolStrip.ClientRectangle);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(_bgColor))
            {
                e.Graphics.FillRectangle(b, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Remove the default ugly border
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (SolidBrush b = new SolidBrush(_selectionColor))
                {
                    e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = _foreColor;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (SolidBrush b = new SolidBrush(_selectionColor))
                    e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            }
            else
            {
                using (SolidBrush b = new SolidBrush(_bgColor))
                    e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected || e.Item.Pressed)
            {
                using (SolidBrush b = new SolidBrush(_selectionColor))
                    e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            }
            else
            {
                using (SolidBrush b = new SolidBrush(_bgColor))
                    e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
            }
        }

    } // end class
}