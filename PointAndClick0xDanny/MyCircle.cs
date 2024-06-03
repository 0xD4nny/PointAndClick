using System.Diagnostics;

namespace PointAndClick0xDanny
{
    class MyCircle
    {
        public Stopwatch LivingTime = new Stopwatch();

        private float _x = -0.5f, _y = -0.5f, _h = 1f, _w = 1f;

        public RectangleF Circle;

        public readonly Color PrimaryColor;
        public readonly Color SecondaryColor;

        public MyCircle(Rectangle gamefield, Random random)
        {
            Circle = new Rectangle(random.Next(gamefield.X + 50, gamefield.X + gamefield.Width - 60), random.Next(gamefield.Y + 50, gamefield.Y + gamefield.Height - 60), 10, 10);
            PrimaryColor = Color.FromArgb(random.Next(32, 255), random.Next(32, 255), random.Next(32, 255));
            SecondaryColor = Color.FromArgb(random.Next(32, 255), random.Next(32, 255), random.Next(32, 255));
            LivingTime.Start();
        }

        public void DrawingCircleInstanz(List<MyCircle> list, ref int life, ref int missHits)
        {
            if (Circle.Height >= 100)
            {
                _w = -_w;
                _h = -_h;
                _x = -_x;
                _y = -_y;
            }

            if (Circle.Height < 10)
            {
                missHits++;
                life--;
                list.Remove(this);
            }

            Circle.Width += _w;
            Circle.Height += _h;
            Circle.X += _x;
            Circle.Y += _y;
        }

    }
}
