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
    public class PanBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.Register(nameof(ScrollViewer), typeof(ScrollViewer), typeof(PanBehavior));
        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseRightButtonDown += OnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp += OnMouseRightButtonUp;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseRightButtonDown -= OnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp -= OnMouseRightButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private Point? _prevPos = null;

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ScrollViewer == null)
                return;

            _prevPos = e.GetPosition(ScrollViewer);
            ScrollViewer.Cursor = Cursors.SizeAll;
            Mouse.Capture(AssociatedObject);
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ScrollViewer == null)
                return;
            if (_prevPos == null)
                return;

            _prevPos = null;
            ScrollViewer.Cursor = Cursors.Arrow;
            Mouse.Capture(null);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollViewer == null)
                return;
            if (_prevPos == null)
                return;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                Point pos = e.GetPosition(ScrollViewer);
                Vector delta = pos - _prevPos.Value;

                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - delta.X);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - delta.Y);

                _prevPos = pos;
            }
        }
    }
}
