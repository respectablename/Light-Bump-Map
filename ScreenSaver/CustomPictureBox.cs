using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ScreenSaver
{
    public class CustomPictureBox : PictureBox
    {
        public CustomPictureBox() : base()
        {
            this.DoubleBuffered = true;
        }
    }
}
