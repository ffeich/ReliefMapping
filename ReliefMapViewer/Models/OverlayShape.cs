using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace ReliefMapViewer.Models
{
    public abstract class OverlayShape
    {
        public Brush Color { get; set; } = Brushes.Yellow;
        public double Scale { get; set; } = 1.0;
    }

    public class CrossShape : OverlayShape
    {
        public Point Center { get; private set; }
        public double Size { get; private set; } = 8;

        public double X1 => (Center.X - Size) * Scale;
        public double X2 => (Center.X + Size) * Scale;
        public double Y1 => (Center.Y - Size) * Scale;
        public double Y2 => (Center.Y + Size) * Scale;
        public double CX => Center.X * Scale;
        public double CY => Center.Y * Scale;

        public CrossShape(Point center, double size, double? scale = null)
        {
            Center = center;
            Size = size;
            if (scale.HasValue)
                Scale = scale.Value;
        }
    }

    public class LineShape : OverlayShape
    {
        public Point P1 { get; private set; }
        public Point P2 { get; private set; }

        public double X1 => P1.X * Scale;
        public double Y1 => P1.Y * Scale;
        public double X2 => P2.X * Scale;
        public double Y2 => P2.Y * Scale;

        public LineShape(Point p1, Point p2, double? scale = null)
        {
            P1 = p1;
            P2 = p2;
            if (scale.HasValue)
                Scale = scale.Value;
        }
    }

    public class CircleShape : OverlayShape
    {
        public Point Center { get; private set; }
        public Point EdgePoint { get; private set; }

        private double _radius;

        public double Radius => _radius * Scale;
        public double Diameter => Radius * 2;
        public double Left => Center.X * Scale - Radius;
        public double Top => Center.Y * Scale - Radius;

        public CircleShape(Point center, Point edgePoint, double? scale = null)
        {
            Center = center;
            EdgePoint = edgePoint;
            if (scale.HasValue)
                Scale = scale.Value;

            _radius = CalculateRadius(Center.X, Center.Y, EdgePoint.X, EdgePoint.Y);
        }

        public static double CalculateRadius(double centerX, double centerY, double edgeX, double edgeY)
        {
            double dx = edgeX - centerX;
            double dy = edgeY - centerY;
            double r = Math.Sqrt(dx * dx + dy * dy);
            return r;
        }
    }
}
