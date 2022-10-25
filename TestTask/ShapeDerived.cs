using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestTask
{
    public class ShapeDerived : ShapeBase
    {

        private readonly GraphicsPath _modelGraphicPath;

        public ShapeDerived(PointF rockPoint) : base(rockPoint)
        {
            _modelGraphicPath = MakePolygonPath(Random.Next(4, 13), 0.6);
        }

        public override GraphicsPath GetPath() 
        {
            var grp = (GraphicsPath)_modelGraphicPath.Clone();
            var mat = new Matrix();

            mat.Translate(Location.X, Location.Y);
            mat.Rotate(FRotation);

            grp.Transform(mat);
            return grp;
        }
    }
}
