using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input;

namespace Yugen.DJ.Renderers
{
    public class TouchPointsRenderer
    {
        private const int maxPoints = 100;
        private readonly Queue<Vector2> points = new Queue<Vector2>();

        public void OnPointerPressed()
        {
            points.Clear();
        }

        public void OnPointerMoved(IList<PointerPoint> intermediatePoints)
        {
            foreach (var point in intermediatePoints)
            {
                if (point.IsInContact)
                {
                    if (points.Count > maxPoints)
                    {
                        points.Dequeue();
                    }

                    points.Enqueue(point.Position.ToVector2());
                }
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            var pointerPointIndex = 0;
            var prev = new Vector2(0, 0);
            const float penRadius = 10;
            foreach (Vector2 p in points)
            {
                if (pointerPointIndex != 0)
                {
                    ds.DrawLine(prev, p, Colors.DarkRed, penRadius * 2);
                }
                prev = p;
                pointerPointIndex++;
            }

            if (points.Count > 0)
                points.Dequeue();
        }
    }
}