using ReliefMapViewer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ReliefMapViewer.ViewModels
{
    public class ReliefMapViewModel : INotifyPropertyChanged
    {
        private ReliefMap Map { get; set; } = null;

        private WriteableBitmap _reliefBitmap;
        public WriteableBitmap ReliefBitmap
        {
            get => _reliefBitmap;
            private set
            {
                if (_reliefBitmap != value)
                {
                    _reliefBitmap = value;
                    OnPropertyChanged(nameof(ReliefBitmap));
                }
            }
        }

        public bool IsLoaded
            => Map != null && Map.HasData;

        private bool _useColor;
        public bool UseColor
        {
            get => _useColor;
            set
            {
                _useColor = value;
                UpdateReliefBitmap();
                OnPropertyChanged(nameof(UseColor));
            }
        }

        public void LoadFromFile(string path)
        {
            Map = ReliefMap.LoadFromAsciiFile(path);

            UpdateReliefBitmap();
        }

        private void UpdateReliefBitmap()
        {
            if (!IsLoaded)
                return;

            ReliefBitmap = UseColor
                ? CreateColoredReliefBitmap(Map)
                : CreateReliefBitmap(Map);
        }

        private WriteableBitmap CreateReliefBitmap(ReliefMap map)
        {
            int width = map.Cols;
            int height = map.Rows;
            int stride = width;     //Gray8

            float min = map.Data.Cast<float>().Min();
            float max = map.Data.Cast<float>().Max();

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
            byte[] pixels = new byte[height * stride];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    float val = map.Data[row, col];
                    byte gray = (byte)((val - min) / (max - min) * 255f);   //normalization [0,1] and scale [0,255]

                    int index = row * stride + col;
                    pixels[index] = gray;
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return bitmap;
        }

        private WriteableBitmap CreateColoredReliefBitmap(ReliefMap map)
        {
            int width = map.Cols;
            int height = map.Rows;
            int stride = width * 4;     //Bgr32

            float min = map.Data.Cast<float>().Min();
            float max = map.Data.Cast<float>().Max();

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            byte[] pixels = new byte[height * stride];

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    float val = map.Data[row, col];
                    float t = (val - min) / (max - min);    //normalization [0,1]

                    byte r, g, b;

                    if (t < 0.5f)
                    {
                        //Blue -> Green
                        //t = 0.0   b = 255 g = 0   -> Blue
                        //t = 0.5   b = 0   g = 255 -> Green

                        float tt = t / 0.5f;
                        r = 0;
                        g = (byte)(tt * 255f);
                        b = (byte)((1 - tt) * 255f);
                    }
                    else
                    {
                        //Green -> Red
                        //t = 0.5   r = 0   g = 255 -> Green
                        //t = 1.0   r = 255 g = 0   -> Red

                        float tt = (t - 0.5f) / 0.5f;
                        r = (byte)(tt * 255f);
                        g = (byte)((1 - tt) * 255f);
                        b = 0;
                    }

                    int index = row * stride + col * 4;
                    pixels[index + 0] = b;  //Blue
                    pixels[index + 1] = g;  //Green
                    pixels[index + 2] = r;  //Red
                    pixels[index + 3] = 0;
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return bitmap;
        }

        public bool IsValidPosition(int row, int col)
        {
            if (!IsLoaded)
                return false;

            return col >= 0 && col < Map.Cols && row >= 0 && row < Map.Rows;
        }

        public float? GetHeight(int row, int col)
        {
            if (!IsLoaded || !IsValidPosition(row, col))
                return null;

            return Map.Data[row, col];
        }

        public (double x, double y) GetAbsXY(int row, int col)
        {
            if (!IsLoaded)
                throw new InvalidOperationException("Map is not loaded");

            return Map.GetAbsXY(row, col);
        }

        public double GetRadius(double centerX, double centerY, double edgeX, double edgeY)
        {
            if (!IsLoaded)
                throw new InvalidOperationException("Map is not loaded");

            return Map.GetRadius(centerX, centerY, edgeX, edgeY);
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
