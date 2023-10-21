using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace IND1
{
    public partial class Form1 : Form
    {
        private List<Point> points = new List<Point>();
        private List<Point> resultUp = new List<Point>();
        private List<Point> resultDown = new List<Point>();

        public Form1()
        {
            InitializeComponent();
        }

        Pen pen = new Pen(Color.Black);
        Brush brush = new SolidBrush(Color.Black);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int i = 0; i < points.Count; i++)
                g.DrawEllipse(pen, points[i].X*1, points[i].Y*1, 1, 1);

            resultUp = resultUp.OrderBy(p => p.X).ToList();
            for (int i = 0; i < resultUp.Count - 1; i++)
                g.DrawLine(pen, resultUp[i], resultUp[i+1]);

            resultDown = resultDown.OrderBy(p => p.X).ToList();
            for (int i = 0; i < resultDown.Count - 1; i++)
                g.DrawLine(pen, resultDown[i], resultDown[i + 1]);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Point position = e.Location;
            points.Add(position);

            this.Refresh();
        }

        private List<Point> upperPoints(Point p1, Point p2)
        {
            List<Point> res = new List<Point>();

            foreach (Point point in points)
            {
                if (resultUp.Any(item => item.X == point.X && item.Y == point.Y))
                    continue;

                float side = (point.Y - p1.Y) * (p2.X - p1.X) -
                                 (point.X - p1.X) * (p2.Y - p1.Y);
                if (side < 0)
                    res.Add(point);
            }
            return res;
        }

        private List<Point> lowerPoints(Point p1, Point p2)
        {
            List<Point> res = new List<Point>();

            foreach (Point point in points)
            {
                if (resultDown.Any(item => item.X == point.X && item.Y == point.Y))
                    continue;

                float side = (point.Y - p1.Y) * (p2.X - p1.X) -
                                 (point.X - p1.X) * (p2.Y - p1.Y);
                if (side > 0)
                    res.Add(point);
            }
            return res;
        }

        private Point upperPoint(List<Point> upperPoints, Point p1, Point p2)
        {
            Point result = new Point();

            float diffX = p2.X - p1.X;
            float diffY = p2.Y - p1.Y;

            double maxDist = double.MinValue;
            foreach (Point point in upperPoints)
            {
                float dist = Math.Abs((diffY * point.X) - (diffX * point.Y) + (p2.X * p1.Y) - (p2.Y * p1.X)) /
                       (float)Math.Sqrt(Math.Pow(diffY, 2) + Math.Pow(diffX, 2));
                if (dist > maxDist)
                {
                    maxDist = dist;
                    result = point;
                }
            }
            return result;
        }

        private Point lowerPoint(List<Point> lowerPoints, Point p1, Point p2)
        {
            Point result = new Point();

            float diffX = p2.X - p1.X;
            float diffY = p2.Y - p1.Y;

            double minDist = double.MinValue;
            foreach (Point point in lowerPoints)
            {
                float dist = Math.Abs((diffY * point.X) - (diffX * point.Y) + (p2.X * p1.Y) - (p2.Y * p1.X)) /
                       (float)Math.Sqrt(Math.Pow(diffY, 2) + Math.Pow(diffX, 2));
                if (dist > minDist)
                {
                    minDist = dist;
                    result = point;
                }
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (points.Count < 3)
                return;

            var temp = points.OrderBy(p => p.X);
            Point p1 = temp.First();
            Point p2 = temp.Last();

            resultUp.Add(p1);
            resultUp.Add(p2);
            resultDown.Add(p1);
            resultDown.Add(p2);

            QuikHullUp(p1, p2);
            QuikHullDown(p1, p2);
            this.Refresh();
            resultUp.Clear();
            resultDown.Clear();
        }

        private void QuikHullUp(Point p1, Point p2)
        {
            var pointsUpper = upperPoints(p1, p2);
            if (pointsUpper.Count == 0)
                return;

            Point p = upperPoint(pointsUpper, p1, p2);
            int index = resultUp.IndexOf(p2);
            resultUp.Insert(index, p);

            QuikHullUp(p1, p);
            QuikHullUp(p, p2);
        }

        private void QuikHullDown(Point p1, Point p2)
        {
            var pointsLower = lowerPoints(p1, p2);
            if (pointsLower.Count == 0)
                return;

            Point p = lowerPoint(pointsLower, p1, p2);
            int index = resultDown.IndexOf(p2);
            resultDown.Insert(index, p);

            QuikHullDown(p1, p);
            QuikHullDown(p, p2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            points = new List<Point>();
            resultUp.Clear(); 
            resultDown.Clear();
            this.Refresh();
        }
    }
}
