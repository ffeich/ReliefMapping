using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace ReliefMapViewer.Behaviors
{
    public class ZoomResetBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty ZoomScaleProperty = DependencyProperty.Register(nameof(ZoomScale), typeof(double), typeof(ZoomResetBehavior),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double ZoomScale
        {
            get => (double)GetValue(ZoomScaleProperty);
            set => SetValue(ZoomScaleProperty, value);
        }

        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register(nameof(ScrollViewer), typeof(ScrollViewer), typeof(ZoomResetBehavior));
        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseDown += OnMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseDown -= OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                ZoomScale = 1.0;

                if (ScrollViewer != null)
                {
                    ScrollViewer.ScrollToHorizontalOffset(0);
                    ScrollViewer.ScrollToVerticalOffset(0);
                }

                e.Handled = true;
            }
        }
    }
}
