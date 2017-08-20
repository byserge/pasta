using System.Windows.Forms;

namespace Pasta.Core
{
    public interface IMouseAware
    {
        void OnMouseDown(MouseEventArgs e);

        void OnMouseUp(MouseEventArgs e);

        void OnMouseMove(MouseEventArgs e);
    }
}
