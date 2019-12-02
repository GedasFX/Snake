using System.Windows.Forms;

namespace Snake.Controls
{
    public sealed class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
        }
    }
}
