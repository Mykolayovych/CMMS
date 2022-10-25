using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestTask
{
    public class Figure : ShapeBase
    {

        private static GraphicsPath _modelGraphicPath = new GraphicsPath();

        public Figure(PointF triPoint) : base(triPoint)
        {
        }

        public override GraphicsPath GetPath()
        {
            RebuildModel();
            var grp = (GraphicsPath)_modelGraphicPath.Clone();

            var mat = new Matrix();
            mat.Translate(Location.X, Location.Y);
            mat.Rotate(FRotation);
            grp.Transform(mat);

            return grp;
        }

        public static void RebuildModel()
        {
            _modelGraphicPath = MakePolygonPath(5, 0);
        }
    }
}
