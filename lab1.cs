using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Dote1: ");
            Point p1 = new Point(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
            Console.WriteLine("Dote2: ");
            Point p2 = new Point(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
            Console.WriteLine("Dote3: ");
            Point p3 = new Point(Convert.ToInt32(Console.ReadLine()), Convert.ToInt32(Console.ReadLine()));
            Triangle t = new Triangle(p1, p2, p3);
            t.print();
            Console.ReadLine();
        }
    }

    class Point
    {
        private int x;
        private int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public double distance(Point p1)
        {
            return Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));
        }


        public void print()
        {
            Console.WriteLine("Dote (" + x + ", " + y + ")");
        }
    }

    class Triangle
    {
        private Point a;
        private Point b;
        private Point c;

        public Triangle(Point a, Point b, Point c)
        {
            if (a != null && b != null && c != null)
            {
              double AB = a.distance(b);
              double AC = a.distance(c);
              double BC = b.distance(c);
                Console.WriteLine(AB + " " + AC + " " + BC);
              
            }
            else throw new Exception("Triangle is not Triangle");
        }

        public double P()
        {
            return a.distance(b) + b.distance(c) + c.distance(a); 
        }

        public double S()
        {
            double p = P() / 2;
            return Math.Sqrt(p * (p - a.distance(b)) * (p - b.distance(c)) * (p - c.distance(a)));
        }

        public void print()
        {
            Console.WriteLine("Triangle (" + a + ", " + b + ", " + c + ")");
            Console.WriteLine("P = "+P()+", S = "+ S());
        }
    }

}
