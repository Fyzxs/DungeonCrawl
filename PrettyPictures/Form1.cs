using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrettyPictures
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.Blue), 10, 10, 20, 20);
            g.FillRectangle(new TextureBrush(new Bitmap("scbw_tiles.png")), 33, 33, 30, 30);
            boop(g);
        }

        private void boop(Graphics g)
        {
            // Create a Bitmap object from a file.
            var myBitmap = new Bitmap("scbw_tiles.png");

            // Clone a portion of the Bitmap object.
            var cloneRect = new Rectangle(33*3, 33*3 + 1, 33, 33);
            var format = myBitmap.PixelFormat;
            var cloneBitmap = myBitmap.Clone(cloneRect, format);

            // Draw the cloned portion of the Bitmap object.
            g.DrawImage(cloneBitmap, 0, 0);
        }
    }
}

