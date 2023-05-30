using PicEdit.Views.Windows;
using System.Linq;
using System.Windows;

namespace PicEdit.Services
{
    public class WindowService : IWindowService
    {
        public void OpenWindow()
        {
            AboutWindow window = new AboutWindow();
            window.ShowDialog();
        }

        public void CloseWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            if (window != null)
            {
                window.Close();
            }
        }
    }
}
