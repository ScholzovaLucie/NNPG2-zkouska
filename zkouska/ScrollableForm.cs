using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zkouska
{
    internal class ScrollableForm : Form
    {
        public ScrollableForm()
        {
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(1000, 1000);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            g.FillRectangle(Brushes.Blue, new Rectangle(50, 50, 200, 100));
        }
    }
}
