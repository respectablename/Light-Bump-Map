using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ScreenSaver
{
    public class Light
    {
        private Point _lightPosition;
        public Point Position
        {
            get { return _lightPosition; }
            set { _lightPosition = value; }
        }

        private Size _lightSize;
        public Size Size
        {
            get { return _lightSize; }
            set { _lightSize = value; }
        }

        private int[,] _lightSource;
        public int[,] Source
        {
            get { return _lightSource; }
            set { _lightSource = value; }
        }

        public enum direction { Right, Left, Up, Down }
        private direction _direction;
        public direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        

        public Light(Point position, Size size)
        {
            _lightPosition = position;
            _lightSize = size;
            GenerateSource();
            _direction = direction.Up;
        }

        private void GenerateSource()
        {
            _lightSource = new int[_lightSize.Width, _lightSize.Height];
            for (int a = 1; a < _lightSize.Height; a++)
            {
                int aa = (_lightSize.Height / 2 - 1 - a) * (_lightSize.Height / 2 - 1 - a);
                for (int b = 1; b < _lightSize.Width; b++)
                {
                    int dist = Convert.ToInt32(Math.Sqrt(((_lightSize.Width / 2 - 1 - b) * (_lightSize.Width / 2 - 1 - b)) + aa));
                    if (dist <= _lightSize.Width / 2 - 1)
                        _lightSource[b, a] = _lightSize.Width / 2 - dist;
                }
            }
        }

        public void UpdatePosition(int speed, Size screen)
        {
            switch (_direction)
            {
                case direction.Up:
                    _lightPosition.Y -= speed;
                    if (_lightPosition.Y < screen.Height * .40)
                    {
                        _direction = direction.Right;
                    }
                    break;
                case direction.Right:
                    _lightPosition.X += speed;
                    if (_lightPosition.X > screen.Width * .90)
                    {
                        _direction = direction.Down;
                    }
                    break;
                case direction.Down:
                    _lightPosition.Y += speed;
                    if (_lightPosition.Y > screen.Height * .70)
                    {
                        _direction = direction.Left;
                    }
                    break;
                case direction.Left:
                    _lightPosition.X -= speed;
                    if (_lightPosition.X < screen.Width * .40)
                    {
                        _direction = direction.Up;
                    }
                    break;
            }

        }
    }
}
