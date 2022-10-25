using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace TestTask
{
    public partial class MainForm : Form
    {
        private readonly List<ShapeBase> _allshapes = new List<ShapeBase>();
        private readonly BufferedGraphicsContext _bgc = new BufferedGraphicsContext();             
        private BufferedGraphics _bg;
        readonly List<Regs> _allRegions = new List<Regs>();
        private int _countMarker;
        private int _countClashes;
        readonly Stopwatch _watch = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();

            MouseDown += Form_MouseDown;                                  
            timerClick.Enabled = true;                                     
            timerClick.Tick += TimerClick_Tick;                            
            Resize += Form_Resize;                                        
            _bg = _bgc.Allocate(CreateGraphics(), DisplayRectangle);  
            ShapeBase.CfRadius = 10;
            Text = @"Pointy Pixel Size : " + ShapeBase.CfRadius.ToString(CultureInfo.InvariantCulture);
            MouseWheel += Form_MouseWheel;                               
            ShapeBase.BaseColor = Color.Green;
        }

        public sealed override Rectangle DisplayRectangle => base.DisplayRectangle;

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_allshapes.Count >= 1) return;

            if (!(ShapeBase.CfRadius + (e.Delta / 100) > 4)) return;
            ShapeBase.CfRadius += e.Delta / 100;
            Text = @"Pointy Pixel Size : " + ShapeBase.CfRadius.ToString(CultureInfo.InvariantCulture);
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            _bg = _bgc.Allocate(CreateGraphics(), this.DisplayRectangle);
        }

        private void TimerClick_Tick(object sender, EventArgs e)
        {
            var gr = _bg.Graphics;
            gr.Clear(Color.FromKnownColor(KnownColor.Control));
            _allshapes.ForEach(x => x.Tick(ClientRectangle));
            _allshapes.ForEach(x => x.IsMarkedToCompare = false);

            for (var i = 0; i < _allshapes.Count; i++)
            {
                var a = new Region(_allshapes[i].GetPath());
                for (var j = i; j < _allshapes.Count; j++)
                {
                    if (!ReferenceEquals(_allshapes[i], _allshapes[j]))
                    {
                        _allshapes[i].Dist(_allshapes[j]);
                        _allshapes[j].Dist(_allshapes[i]);

                        if (_allshapes[i].IsMarkedToCompare)
                        {
                            _countMarker++;
                            var b = new Region(_allshapes[j].GetPath());
                            var inter = a.Clone();
                            inter.Intersect(b);

                            if (!inter.IsEmpty(gr))
                            {
                                var myRegion = new Regs(inter);
                                _allRegions.Add(myRegion);
                                _allshapes[i].IsMarkedForDeath = true;
                                _allshapes[j].IsMarkedForDeath = true;

                                if (_countMarker != 1)
                                {
                                    _countClashes++;
                                }
                                _countMarker = 0;
                            }
                        }
                    }
                }
            }

            _allRegions.ForEach(x => gr.FillRegion(new SolidBrush(Color.Black), x.IntRegion));
            _allRegions.RemoveAll(x => x.Stop.ElapsedMilliseconds > 2000);
            _allshapes.RemoveAll(x => x.IsMarkedForDeath);
            TimeSpan t = TimeSpan.FromSeconds(_watch.Elapsed.Seconds);

            string timeStopwatch = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                t.Hours,
                t.Minutes,
                t.Seconds,
                t.Milliseconds);
            gr.DrawString($"Total Figure: {_allshapes.Count}\nTotal Clashes: {_countClashes}\nSeconds: {timeStopwatch}", new Font("Ariel", 15), new SolidBrush(Color.Black),
                ClientRectangle.Width-215, ClientRectangle.Height-80);
            _allshapes.ForEach(x => x.Render(gr));
            _bg.Render();

        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _watch.Start();
                var newFigure = new Figure(e.Location);
                _allshapes.Add(newFigure);
            }

            if (e.Button == MouseButtons.Right)
            {
                _watch.Start();
                var newShapeDerived = new ShapeDerived(e.Location);
                _allshapes.Add(newShapeDerived);
            }

            if (e.Button == MouseButtons.Left && ModifierKeys == Keys.Control)
            {
                _watch.Stop();
                _allshapes.Clear();
            }
        }

       
    }
}
