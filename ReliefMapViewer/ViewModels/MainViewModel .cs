using Microsoft.Win32;
using ReliefMapViewer.Behaviors;
using ReliefMapViewer.Commands;
using ReliefMapViewer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReliefMapViewer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public RadiusOverlayViewModel RadiusOverlay { get; } = new RadiusOverlayViewModel();

        public ReliefMapViewModel ReliefMap { get; } = new ReliefMapViewModel();

        private double _zoomScale = 1.0;
        public double ZoomScale
        {
            get => _zoomScale;
            set
            {
                if (_zoomScale != value)
                {
                    _zoomScale = value;
                    OnPropertyChanged(nameof(ZoomScale));
                    OnZoomScaleChanged();
                }
            }
        }

        private string _fileInfo = string.Empty;
        public string FileInfo
        {
            get => _fileInfo;
            private set
            {
                _fileInfo = value;
                OnPropertyChanged(nameof(FileInfo));
            }
        }

        private string _cursorInfo = string.Empty;
        public string CursorInfo
        {
            get => _cursorInfo;
            private set
            {
                _cursorInfo = value;
                OnPropertyChanged(nameof(CursorInfo));
            }
        }

        private string _radiusInfo = string.Empty;
        public string RadiusInfo
        {
            get => _radiusInfo;
            private set
            {
                _radiusInfo = value;
                OnPropertyChanged(nameof(RadiusInfo));
            }
        }

        private string _zoomInfo = string.Empty;
        public string ZoomInfo
        {
            get => _zoomInfo;
            private set
            {
                _zoomInfo = value;
                OnPropertyChanged(nameof(ZoomInfo));
            }
        }

        public ICommand LoadReliefMapCommand { get; }
        public ICommand CursorMoveCommand { get; }
        public ICommand RadiusAddCommand { get; }

        public MainViewModel()
        {
            LoadReliefMapCommand = new RelayCommand(LoadReliefMap);
            CursorMoveCommand = new RelayCommand<Point>(OnCursorMove);
            RadiusAddCommand = new RelayCommand<RadiusAddArgs>(OnRadiusAdd);

            UpdateFileInfo(null);
        }

        private void LoadReliefMap(object parameter)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() ?? false)
            {
                ReliefMap.LoadFromFile(openFileDialog.FileName);

                RadiusOverlay.Clear();
                ZoomScale = 1.0;
                UpdateRadiusInfo(null, null);
                UpdateZoomInfo();
                UpdateFileInfo(openFileDialog.FileName);
            }
        }

        private void OnRadiusAdd(RadiusAddArgs args)
        {
            UpdateRadiusOverlay(args.CircleCenter, args.CircleEdgePoint ?? args.NewCircleEdgePoint);
            UpdateRadiusInfo(args.CircleCenter, args.CircleEdgePoint);
        }

        private void OnCursorMove(Point pos)
        {
            UpdateCursorInfo(pos);
        }

        public void UpdateCursorInfo(Point pos)
        {
            if (!ReliefMap.IsLoaded)
            {
                CursorInfo = string.Empty;
                return;
            }

            int col = (int)pos.X;
            int row = (int)pos.Y;

            if (!ReliefMap.IsValidPosition(row, col))
            {
                CursorInfo = string.Empty;
                return;
            }

            float? height = ReliefMap.GetHeight(row, col);
            var (x, y) = ReliefMap.GetAbsXY(row, col);

            CursorInfo = $"X={x:G} Y={y:G} Height={height:G}";
        }

        private void UpdateRadiusInfo(Point? circleCenter, Point? circleEdgePoint)
        {
            if (!ReliefMap.IsLoaded)
            {
                RadiusInfo = string.Empty;
            }

            if (circleCenter == null && circleEdgePoint == null)
            {
                RadiusInfo = "Select the center of the circle";
            }
            else if (circleCenter != null && circleEdgePoint == null)
            {
                RadiusInfo = "Select the edge of the circle";
            }
            else
            {
                var center = circleCenter.Value;
                var edge = circleEdgePoint.Value;

                var (x, y) = ReliefMap.GetAbsXY((int)center.Y, (int)center.X);
                var r = ReliefMap.GetRadius(center.X, center.Y, edge.X, edge.Y);

                RadiusInfo = $"X={x:F2} Y={y:F2} Radius={r:F2}";
            }
        }

        public void UpdateFileInfo(string fileName)
        {
            var fileInfo = Assembly.GetEntryAssembly()?.GetName().Name;

            if (!string.IsNullOrEmpty(fileName))
                fileInfo += $" - {fileName}";
            FileInfo = fileInfo;
        }

        public void UpdateZoomInfo()
        {
            ZoomInfo = $"Zoom={(ZoomScale * 100):F0}%";
        }

        private void UpdateRadiusOverlay(Point? circleCenter, Point? circleEdgePoint)
        {
            RadiusOverlay.UpdateRadius(circleCenter, circleEdgePoint, ZoomScale);
        }

        private void OnZoomScaleChanged()
        {
            RadiusOverlay.UpdateScale(ZoomScale);
            UpdateZoomInfo();
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
