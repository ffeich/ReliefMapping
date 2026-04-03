using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReliefMapViewer.Behaviors
{
    public class ZoomBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ZoomBehavior));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty ZoomScaleProperty = DependencyProperty.Register(nameof(ZoomScale), typeof(double), typeof(ZoomBehavior),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double ZoomScale
        {
            get => (double)GetValue(ZoomScaleProperty);
            set => SetValue(ZoomScaleProperty, value);
        }

        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register(nameof(ScrollViewer), typeof(ScrollViewer), typeof(ZoomBehavior));
        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseWheel += OnMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseWheel -= OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double minZoom = 0.5;
            const double maxZoom = 10.0;
            const double zoomFactor = 1.1;

            double delta = e.Delta > 0 ? zoomFactor : 1 / zoomFactor;
            double newScale = ZoomScale * delta;

            if (newScale < minZoom || newScale > maxZoom)
                return;

            if (ScrollViewer != null)
            {
                Point posImage = e.GetPosition(AssociatedObject);
                Point posScrollViewer = e.GetPosition(ScrollViewer);

                double newX = posImage.X * newScale;
                double newY = posImage.Y * newScale;

                ScrollViewer.UpdateLayout();
                ScrollViewer.ScrollToHorizontalOffset(newX - posScrollViewer.X);
                ScrollViewer.ScrollToVerticalOffset(newY - posScrollViewer.Y);
            }

            ZoomScale = newScale;

            e.Handled = true;
        }
    }
}
