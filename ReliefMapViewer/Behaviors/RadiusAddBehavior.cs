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
    public class RadiusAddBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(RadiusAddBehavior));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private Point? _circleCenter = null;
        private Point? _circleEdgePoint = null;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_circleCenter.HasValue && !_circleEdgePoint.HasValue)
            {
                var newCircleEdgePoint = e.GetPosition(AssociatedObject);

                Command?.Execute(new RadiusAddArgs(_circleCenter, _circleEdgePoint, newCircleEdgePoint));
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(AssociatedObject);

            if (_circleCenter == null)
            {
                _circleCenter = p;
            }
            else if (_circleEdgePoint == null)
            {
                _circleEdgePoint = p;
            }
            else
            {
                _circleCenter = null;
                _circleEdgePoint = null;
            }

            Command?.Execute(new RadiusAddArgs(_circleCenter, _circleEdgePoint));
        }
    }

    public class RadiusAddArgs
    {
        public Point? CircleCenter { get; private set; }
        public Point? CircleEdgePoint { get; private set; }
        public Point? NewCircleEdgePoint { get; private set; }

        public RadiusAddArgs(Point? circleCenter, Point? circleEdgePoint, Point? newCircleEdgePoint = null)
        {
            CircleCenter = circleCenter;
            CircleEdgePoint = circleEdgePoint;
            NewCircleEdgePoint = newCircleEdgePoint;
        }
    }
}
