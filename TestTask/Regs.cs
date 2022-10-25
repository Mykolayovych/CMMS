using System.Diagnostics;
using System.Drawing;

namespace TestTask
{
    public class Regs
    {
        public Regs(Region mRegion)
        {
            IntRegion = mRegion;
            Stop = new Stopwatch();
            Stop.Start();
        }

        public Region IntRegion { get; set; }

        public Stopwatch Stop { get; set; }
    }
}
