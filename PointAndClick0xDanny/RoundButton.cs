using System.Drawing.Drawing2D;

namespace PointAndClick0xDanny
{
    public partial class RoundButton : Button
    {
        private bool isHovered = false;
        private bool isPressed = false;

        public Region? ButtonRegion;
        private GraphicsPath? path;

        public Color EdgeColor;
        public Color HoverColor;
        public Color PressedColor;

        public int EdgeSize = 1;
        public int ArcLength = 10;
        public RoundButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            AutoSize = true;
            TextAlign = ContentAlignment.MiddleCenter;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;

            MouseEnter += new EventHandler(RoundButton_MouseEnter);
            MouseLeave += new EventHandler(RoundButton_MouseLeave);
            MouseDown += new MouseEventHandler(RoundButton_MouseDown);
            MouseUp += new MouseEventHandler(RoundButton_MouseUp);
        }

        protected override bool ShowFocusCues => false;

        protected override void OnGotFocus(EventArgs e)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {

        }

        private void RoundButton_MouseEnter(object sender, EventArgs e)
        {
            isHovered = true;
            Invalidate();
        }

        private void RoundButton_MouseLeave(object sender, EventArgs e)
        {
            isHovered = false;
            Invalidate();
        }

        private void RoundButton_MouseDown(object sender, MouseEventArgs e)
        {
            isPressed = true;
            Invalidate();
        }

        private void RoundButton_MouseUp(object sender, MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (path.IsVisible(e.Location))
                base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (path is null)
                path = GetButtonEdgePath();

            base.OnPaint(e);

            using (Pen myPen = new Pen(EdgeColor, EdgeSize))
            {
                if (path is null)
                    throw new Exception("path can't be null.");

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Color buttonColor = BackColor;
                if (isHovered|| Focused)
                    buttonColor = EdgeColor;
                else if (isPressed)
                    buttonColor = Color.FromArgb(EdgeColor.R / 2, EdgeColor.G / 2, EdgeColor.B / 2);

                if (isHovered || isPressed)
                    e.Graphics.FillPath(new SolidBrush(buttonColor), path);

                e.Graphics.DrawPath(myPen, path);

                if (isPressed || isHovered)
                    e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), ClientRectangle, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
            }
        }

        private GraphicsPath GetButtonEdgePath()
        {
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(0, 0, ArcLength, Height - EdgeSize, 90, 180);
            path.AddArc(Width - ArcLength - EdgeSize, 0, ArcLength, Height - EdgeSize, 270, 180);
            path.CloseFigure();
            return path;
        }

    }
}
