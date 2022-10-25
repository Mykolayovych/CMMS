using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestTask
{
    public abstract class ShapeBase
    {
        protected float FRotation;             
        protected float FRotationIncrement;    
        protected float FxSpeed;             
        protected float FySpeed;
        protected static Random Random = new Random();   

        public static float CfRadius { get; set; }

        public static Color BaseColor { get; set; }

        public bool IsMarkedForDeath { get; set; }

        public bool IsMarkedToCompare { get; set; }

        public PointF Location { get; set; }

        public void Dist(ShapeBase comparison)
        {
            var distance = Math.Sqrt(Math.Pow(Location.X - comparison.Location.X, 2) + Math.Pow(Location.Y - comparison.Location.Y, 2));

            if (distance < CfRadius * 2)
                IsMarkedToCompare = true;
        }

        public abstract GraphicsPath GetPath();

        public void Render(Graphics gr)
        {
            if (IsMarkedToCompare)
                gr.DrawEllipse(new Pen(Color.Black), Location.X - CfRadius, Location.Y - CfRadius, CfRadius * 2, CfRadius * 2);
            gr.FillPolygon(new SolidBrush(BaseColor), GetPath().PathPoints);
        }

        public void Tick(Rectangle clientRectangle)
        {
            var pointy = new PointF(Location.X + FxSpeed, Location.Y + FySpeed);

            if (pointy.X >= clientRectangle.Width)
            {
                pointy.X = clientRectangle.Width;
                FxSpeed *= -1;
            }

            if (pointy.X < 0)
            {
                pointy.X = 0;
                FxSpeed *= -1;
            }

            if (pointy.Y >= clientRectangle.Height)
            {
                pointy.Y = clientRectangle.Height;
                FySpeed *= -1;
            }

            if (pointy.Y < 0)
            {
                pointy.Y = 0;
                FySpeed *= -1;
            }

            Location = pointy;
            FRotation += FRotationIncrement;

        }

        public static GraphicsPath MakePolygonPath(int numPoints, double variance)
        {
            var mylist = new List<PointF>();
            var grpath = new GraphicsPath();

            double minRadius = CfRadius * (1 - variance);

            for (double i = 0; i < Math.PI * 2; i += Math.PI * 2 / numPoints)
            {
                double newRadius = Random.NextDouble() * (CfRadius - minRadius) + minRadius;
                mylist.Add(new PointF((float)(Math.Sin(i) * newRadius), (float)(Math.Cos(i) * newRadius)));
            }

            grpath.AddPolygon(mylist.ToArray());
            return grpath;
        }

        protected ShapeBase(PointF pos)
        {
            Location = pos;                                    
            FRotation = 0;                                    
            FRotationIncrement = (float)(Random.NextDouble() * 6 - 3);       
            FxSpeed = (float)(Random.NextDouble() * 10 - 2.5);                
            FySpeed = (float)(Random.NextDouble() * 15 - 2.5);                
        }
    }
}
