using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FruitSortingVtest1._0
{
    public partial class CircleShape : UserControl
    {
        public CircleShape()
        {
            SetStyle(ControlStyles.DoubleBuffer
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.ResizeRedraw
            | ControlStyles.UserPaint
            | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen blackPen = new Pen(Color.Black, 1))
            {
                e.Graphics.DrawEllipse(blackPen, 0, 0, this.Width - 1, this.Height - 1);
            }
            base.OnPaint(e);
        }
    }
}
