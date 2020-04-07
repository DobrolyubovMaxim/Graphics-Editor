using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
