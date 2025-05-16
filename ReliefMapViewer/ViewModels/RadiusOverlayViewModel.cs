using ReliefMapViewer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReliefMapViewer.ViewModels
{
    public class RadiusOverlayViewModel
    {
        public ObservableCollection<OverlayShape> Shapes { get; } = new ObservableCollection<OverlayShape>();

        public void UpdateRadius(Point? circleCenter, Point? circleEdgePoint, double scale)
        {
            Shapes.Clear();

            if (circleCenter.HasValue)
            {
                Shapes.Add(new CrossShape(circleCenter.Value, 8, scale));

                if (circleEdgePoint.HasValue)
                {
                    Shapes.Add(new CrossShape(circleEdgePoint.Value, 5, scale));
                    Shapes.Add(new LineShape(circleCenter.Value, circleEdgePoint.Value, scale));
                    Shapes.Add(new CircleShape(circleCenter.Value, circleEdgePoint.Value, scale));
                }
            }
        }

        public void UpdateScale(double scale)
        {
            var items = Shapes.ToList();
            Shapes.Clear();

            foreach (var s in items)
            {
                s.Scale = scale;
                Shapes.Add(s);
            }
        }

        public void Clear()
            => Shapes.Clear();
    }
}
