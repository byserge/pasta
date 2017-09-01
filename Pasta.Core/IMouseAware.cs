using System.Windows.Forms;

namespace Pasta.Core
{
    public interface IMouseAware
    {
        void OnMouseDown(MouseAwareArgs e);

        void OnMouseUp(MouseAwareArgs e);

        void OnMouseMove(MouseAwareArgs e);
    }
}
