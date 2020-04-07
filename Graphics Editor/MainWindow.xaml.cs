using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Graphics_Editor
{
    public partial class MainWindow : Window
    {
        Bitmap current = null;
        Bitmap original = null;
        Cursor stdCursor;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Обработка кликов

        private void OpenImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();

            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
            openDlg.InitialDirectory = dir.Parent.Parent.FullName + "\\Examples";

            openDlg.ShowDialog();

            if (openDlg.FileName != "")
            {
                original = new Bitmap(openDlg.FileName);
                current = new Bitmap(original);
                Image.Source = BitmapToBitmapImage(current);
            }
        }

        private void SaveImg_Click(object sender, RoutedEventArgs e)
        {
            if (current != null)
            {
                SaveFileDialog saveDlg = new SaveFileDialog();

                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                saveDlg.InitialDirectory = dir.Parent.Parent.FullName + "\\Examples";
                saveDlg.ShowDialog();

                if (saveDlg.FileName != "")
                {
                    current.Save(saveDlg.FileName);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (original != null)
            {
                current = new Bitmap(original);
                Image.Source = BitmapToBitmapImage(original);
            }
        }

        private void noiceSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                current = AddGaussianNoise(current, noiceSlider.Value);
                Image.Source = BitmapToBitmapImage(current);
                noiceSlider.Value = 0;
                this.Cursor = stdCursor;
            }
        }

        private void blurSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                if (blurSlider.Value != 0)
                {
                    current = Blur(current, (int)blurSlider.Value);
                    Image.Source = BitmapToBitmapImage(current);
                    blurSlider.Value = 0;
                }
                this.Cursor = stdCursor;
            }
        }

        private void Gistogramma_Click(object sender, RoutedEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                ModalWindow newTaskWindow = new ModalWindow(Gistogramm(current));
                this.Cursor = stdCursor;
                newTaskWindow.Owner = this;
                newTaskWindow.ShowDialog();
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                current = LinearFilter(current);
                Image.Source = BitmapToBitmapImage(current);
                this.Cursor = stdCursor;
            }
        }

        private void rotateSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                if (rotateSlider.Value != 0)
                {
                    current = Rotate(current, (int)rotateSlider.Value);
                    Image.Source = BitmapToBitmapImage(current);
                    rotateSlider.Value = 0;
                }
                this.Cursor = stdCursor;
            }
        }

        private void scalePlusSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                if (scalePlusSlider.Value != 100)
                {
                    //current = SosedScale(current, (int)scalePlusSlider.Value);
                    current = Scale(current, (int)scalePlusSlider.Value);
                    Image.Source = BitmapToBitmapImage(current);
                    scalePlusSlider.Value = 100;
                }
                this.Cursor = stdCursor;
            }
        }

        private void scaleMinusSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (current != null)
            {
                stdCursor = this.Cursor;
                this.Cursor = Cursors.Wait;
                if (scaleMinusSlider.Value != 100)
                {
                    //current = SosedScale(current, (int)scaleMinusSlider.Value);
                    current = Scale(current, (int)scaleMinusSlider.Value);
                    Image.Source = BitmapToBitmapImage(current);
                    scaleMinusSlider.Value = 100;
                }
                this.Cursor = stdCursor;
            }
        }

        #endregion

        #region Обработка изображений

        private Bitmap Blur(Bitmap bmp, int radius)
        {
            Bitmap blurBmp = new Bitmap(bmp.Width, bmp.Height);
            System.Drawing.Color color = new System.Drawing.Color();
            int r, g, b, i, j, k, l;
            int counter;
            for (i = 0; i < bmp.Width; i++)
                for (j = 0; j < bmp.Height; j++)
                {
                    r = 0; g = 0; b = 0; counter = 0;
                    for (k = i - radius; k < i + radius + 1; k++)
                        for (l = j - radius; l < j + radius + 1; l++)
                            if (k >= 0 && l >= 0 && k < bmp.Width && l < bmp.Height)
                            {
                                color = bmp.GetPixel(k, l);
                                r += color.R;
                                g += color.G;
                                b += color.B;
                                counter++;
                            }

                    blurBmp.SetPixel(i, j, System.Drawing.Color.FromArgb(r / counter, g / counter, b / counter));
                }
            return blurBmp;
        }

        private Bitmap AddGaussianNoise(Bitmap bmp, double noisePower)
        {
            //var res = (Bitmap)bmp.Clone();
            Bitmap res = new Bitmap(bmp);
            Random rnd = new Random();

            using (var wr = new ImageWrapper(res))
                foreach (var p in wr)
                {
                    var c = wr[p];
                    var noise = (rnd.NextDouble() + rnd.NextDouble() - 1) * noisePower;
                    wr.SetPixel(p, c.R + noise, c.G + noise, c.B + noise);
                }

            return res;
        }

        public Bitmap[] Gistogramm(Bitmap bmp)
        {
            Bitmap[] barChart = new Bitmap[4];
            if (bmp != null)
            {
                int width = 256, height = 256;
                barChart[0] = new Bitmap(width, height);
                barChart[1] = new Bitmap(width, height);
                barChart[2] = new Bitmap(width, height);
                barChart[3] = new Bitmap(width, height);
                int[] R = new int[256];
                int[] G = new int[256];
                int[] B = new int[256];
                int[] A = new int[256];
                int i, j;
                System.Drawing.Color color;
                for (i = 0; i < bmp.Width; ++i)
                    for (j = 0; j < bmp.Height; ++j)
                    {
                        color = bmp.GetPixel(i, j);
                        ++R[color.R];
                        ++G[color.G];
                        ++B[color.B];
                        ++A[(color.R + color.G + color.B) / 3];
                    }
                int max = 0;

                //это ставит верхнюю границу чтобы уместилось всё, но так если будет одна большая пика, 
                //то ничего не будет видно кроме нее
                //for (i = 0; i < 256; ++i)
                //{
                //    if (R[i] > max)
                //        max = R[i];
                //    if (G[i] > max)
                //        max = G[i];
                //    if (B[i] > max)
                //        max = B[i];
                //    if (A[i] > max)
                //        max = A[i];
                //}

                //ставит верхнюю границу в 2.5 средних арифметических
                for (i = 0; i < 256; i++)
                    max += G[i] / 256;
                max = (int)(max * 3); //2, 2.5, дисперсия?

                double point = ((double)max) / height;

                for (i = 0; i < width; ++i)
                {
                    for (j = height - 1; j > height - R[i] / point; --j)
                    {
                        if (j > 0)
                            barChart[0].SetPixel(i, j, System.Drawing.Color.Red);
                    }
                    for (j = height - 1; j > height - G[i] / point; --j)
                    {
                        if (j > 0)
                            barChart[1].SetPixel(i, j, System.Drawing.Color.Green);
                    }
                    for (j = height - 1; j > height - B[i] / point; --j)
                    {
                        if (j > 0)
                            barChart[2].SetPixel(i, j, System.Drawing.Color.Blue);
                    }
                    for (j = height - 1; j > height - A[i] / point; --j)
                    {
                        if (j > 0)
                            barChart[3].SetPixel(i, j, System.Drawing.Color.Black);
                    }
                }
            }
            else
            {
                barChart[0] = new Bitmap(1, 1);
                barChart[1] = new Bitmap(1, 1);
                barChart[2] = new Bitmap(1, 1);
                barChart[3] = new Bitmap(1, 1);
            }

            return barChart;
        }

        private Bitmap Rotate(Bitmap image, float angle)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            var w = (image.Width & 1) == 0 ? image.Width + 1 : image.Width;
            var h = (image.Height & 1) == 0 ? image.Height + 1 : image.Height;
            //центр изображения
            var center = new System.Drawing.Point(w / 2, h / 2);
            //Границы изображения
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            using (var m = new System.Drawing.Drawing2D.Matrix())
            {
                
                System.Drawing.Point[] pts = new System.Drawing.Point[1];
                m.Translate(center.X, center.Y);
                m.Rotate(-angle);
                //m.Invert();
                for (int x = 0; x < image.Width; x++)
                    for (int y = 0; y < image.Height; y++)
                    {
                        
                        //Вектор к пикселу
                        pts[0] = new System.Drawing.Point(x - center.X, y - center.Y);
                        m.TransformPoints(pts);
                        if (!rect.Contains(pts[0]))
                        {
                            continue;
                        }
                        //bmp.SetPixel(pts[0].X, pts[0].Y, image.GetPixel(x, y));
                        bmp.SetPixel(x, y, image.GetPixel(pts[0].X, pts[0].Y));

                    }
                for (int x = 0; x < bmp.Width; x++)
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        if (bmp.GetPixel(x, y).A == 0)
                           bmp.SetPixel(x, y, System.Drawing.Color.White);
                    }

            }
            return bmp;
        }

        private Bitmap SosedScale(Bitmap bmp, double multiplier)
        {
            if (multiplier == 100) return bmp;
            if (multiplier == 0) return new Bitmap(bmp, 1, 1);

            double coef = multiplier / 100;
            Bitmap newBmp;
            if ((int)(bmp.Width * coef) == 0 || (int)(bmp.Height * coef) == 0)
                newBmp = new Bitmap(1, 1);
            else
                newBmp = new Bitmap((int)(bmp.Width * coef), (int)(bmp.Height * coef));

            for (int i = 0; i < newBmp.Width; i++)
                for (int j = 0; j < newBmp.Height; j++)
                    newBmp.SetPixel(i, j, 
                        bmp.GetPixel((int)(i / coef), (int)(j / coef)));

            return newBmp;
        }

        private Bitmap Scale(Bitmap bmp, double multiplier)
        {
            return new Bitmap(bmp,
                (int)(multiplier / 100 * bmp.Width), (int)(multiplier / 100 * bmp.Height));
        }

        private Bitmap LinearFilter(Bitmap bmp)
        {
            Bitmap filtredBmp = new Bitmap(bmp.Width, bmp.Height);
            System.Drawing.Color color = new System.Drawing.Color();
            int r, g, b, i, j, k, l, counter;
            for (i = 0; i < bmp.Width; i++)
                for (j = 0; j < bmp.Height; j++)
                {
                    r = 0; g = 0; b = 0; counter = 0;

                    for (k = i - 1; k < i + 1 + 1; k++)
                        for (l = j - 1; l < j + 1 + 1; l++)
                            if (k >= 0 && l >= 0 && k < bmp.Width && l < bmp.Height)
                            {
                                color = bmp.GetPixel(k, l);
                                //      |1 2 1|                   |2 1|
                                //(1/16)|2 4 2|  (1/9)|4 2| (1/12)|4 2|
                                //      |1 2 1|       |2 1|       |2 1|
                                if (k == i && l == j)
                                {
                                    counter += 4;
                                    r += 4 * color.R;
                                    g += 4 * color.G;
                                    b += 4 * color.B;
                                }
                                else if (k == i || l == j)
                                {
                                    counter += 2;
                                    r += 2 * color.R;
                                    g += 2 * color.G;
                                    b += 2 * color.B;
                                }
                                else
                                {
                                    counter += 1;
                                    r += color.R;
                                    g += color.G;
                                    b += color.B;
                                }

                            }

                    filtredBmp.SetPixel(i, j, System.Drawing.Color.FromArgb(r / counter, g / counter, b / counter));
                }
            return filtredBmp;
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        #endregion

    }
}
