using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScreenSaver
{
    public partial class ScreenSaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        private Point mouseLocation;
        private bool previewMode = false;

        public ScreenSaverForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
            
        }
        public ScreenSaverForm()
        {
            InitializeComponent();
        }

        public ScreenSaverForm(IntPtr PreviewWndHandle)
        {
            InitializeComponent();

            // Set the preview window as the parent of this window
            SetParent(this.Handle, PreviewWndHandle);

            // Make this a child window so it will close when the parent dialog closes
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000));

            // Place our window inside the parent
            Rectangle ParentRect;
            GetClientRect(PreviewWndHandle, out ParentRect);
            Size = ParentRect.Size;
            Location = new Point(0, 0);


            previewMode = true;
        }
        int[,] bump;
        Light light;

        private void ScreenSaverForm_Load(object sender, EventArgs e)
        {                  
	        Cursor.Hide();
	        TopMost = true;

            bump = new int[this.Width, this.Height];
            light = new Light(new Point(Convert.ToInt32(this.Width * 0.40), Convert.ToInt32(this.Height * 0.50)), new Size(this.Height / 3, this.Height / 3));
            rand = new Random();
            

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    bump[x, y] = rand.Next(1024);
                }
            }

            for (int aa = 0; aa < 5; aa++)
            {
                for (int a = 1; a < this.Height - 1; a++)
                {
                    for (int b = 1; b < this.Width - 1; b++)
                    {
                        bump[b, a] = (bump[b, a + 1] + bump[b + 1, a] + bump[b - 1, a] + bump[b, a - 1]) / 4;
                    }
                }
            }
            for (int aa = 0; aa < this.Width; aa++)
            {
                bump[aa, 0] = 512;
                bump[aa, 1] = 512;
                bump[aa, this.Height - 2] = 512;
                bump[aa, this.Height - 1] = 512;
            }
            for (int aa = 0; aa < this.Height; aa++)
            {
                bump[0, aa] = 512;
                bump[1, aa] = 512;
                bump[this.Width - 2, aa] = 512;
                bump[this.Width - 1, aa] = 512;
            }
            

            moveTimer.Interval = 10;
            moveTimer.Tick += new EventHandler(moveTimer_Tick);
            moveTimer.Start();
            
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty)
            {
                if (Math.Abs(mouseLocation.X - e.X) > 5 || Math.Abs(mouseLocation.Y - e.Y) > 5)
                {
                    Application.Exit();
                }
            }
            mouseLocation = e.Location;
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void ScreenSaverForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!previewMode)
            {
                Application.Exit();
            }
        }

        private void moveTimer_Tick(object sender, EventArgs e)
        {
            int movement = 2;
            //this.BackgroundImage.Save("c:\\tmp.bmp");
            light.UpdatePosition(movement, pictureBox1.Size);
            pictureBox1.Invalidate();
            
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black,1);
            e.Graphics.DrawRectangle(pen, 0, 0, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawImage(makeImage(), 0, 0);
        }

        Random rand;
        
        unsafe private Bitmap makeImage()
        {
            Bitmap bitmap = new Bitmap(this.Width, this.Height);
            FastBitmap output = new FastBitmap(bitmap);
            output.LockImage();
            int a,b;
            int c;
            int xx,yy,my,mx;
            int starty = light.Position.Y-light.Size.Height;
            int endy = starty + light.Size.Height;
            for (a = 0; a < this.Height - 1; a++)
            {
                if (a < 0)
                    continue;
                int startx = light.Position.X - light.Size.Width;
                int endx = startx + light.Size.Width;
                for (b = 0; b < this.Width - 1; b++)
                {
                    if (b < 0)
                        continue;

                    my = bump[b, a];
                    mx = my;
                    yy = light.Position.Y - a + my - bump[b, a + 1];
                    xx = light.Position.X - b + mx - bump[b + 1, a];
                    if (yy > 0 && yy < light.Size.Height &&
                    xx > 0 && xx < light.Size.Width)
                    {
                        c = light.Source[xx, yy];
                    }
                    else
                    {
                        c = 0;
                    }
                    if (c > 255)
                        c = 255;
                    if (c < 10) continue;
                    output.SetPixel(b, a, Color.FromArgb(0, c, 0));
                    //bitmap.SetPixel(b, a, Color.FromArgb(c, c, c));
                }
            }
            output.UnlockImage();
            return bitmap;
        }

    

    }
}