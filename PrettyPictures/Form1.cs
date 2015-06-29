using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrettyPictures.Spites;
using PrettyPictures.Spites.Map;

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
            g.DrawImage(SpriteAccess.StaticFloorTile.GetBitmap(), 0, 0);
        }
    }
}

