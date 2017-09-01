using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pasta.Core
{
    [Serializable]
    public class MouseAwareArgs
    {
         public MouseButtons Button { get; private set; }
        
        public int Clicks { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Delta { get; private set; }

        public Point Location => new Point(X, Y);

        public MouseAwareArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        public MouseAwareArgs(MouseEventArgs mouseArgs) 
            : this(mouseArgs.Button, mouseArgs.Clicks, mouseArgs.X, mouseArgs.Y, mouseArgs.Delta)
        {
        }
    }
}
