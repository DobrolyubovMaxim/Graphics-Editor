using System.Drawing;
using System.Windows;

namespace Graphics_Editor
{
    public partial class ModalWindow : Window
    {
        public ModalWindow()
        {
            InitializeComponent();
        }
        public ModalWindow(Bitmap[] gistorgamms)
        {
            InitializeComponent();

            image1.Source = MainWindow.BitmapToBitmapImage(gistorgamms[0]);
            image2.Source = MainWindow.BitmapToBitmapImage(gistorgamms[1]);
            image3.Source = MainWindow.BitmapToBitmapImage(gistorgamms[2]);
            image4.Source = MainWindow.BitmapToBitmapImage(gistorgamms[3]);
            this.Title = "Гистограмма";
        }
    }
}
