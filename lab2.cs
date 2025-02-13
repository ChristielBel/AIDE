using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Avto car = new Avto("Toyota", "AB1234CD", 50000);
            car.Drive(100);

            Taxi taxi = new Taxi("Honda", "XY9812XY", 30000, "Иван Иванов", 3.5);

            taxi.Drive(100);

            taxi.Print();

            Console.ReadLine();
        }
    }

    public class Avto
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public double Mileage { get; set; }
  
        public Avto(string name, string number, double mileage)
        {
            Name = name;
            Number = number;
            Mileage = mileage;
        }

        public virtual void Print()
        {
            Console.WriteLine("Автомобиль: " + Name + " Номер: " + Number + " Пробег: " + Mileage);
        }

        public virtual void Drive(double distance)
        {
            Mileage += distance;
            Console.WriteLine("Автомобиль " + Name + " проехал " + distance + " км. Общий пробег: " + Mileage);
        }
    }

    public class Taxi : Avto
    {
        public string Fio { get; set; }
        public double FareRate { get; set; }


        public Taxi(string name, string number, double mileage, string fio, double farerate): base(name, number, mileage)
        {
            Fio = fio;
            FareRate = farerate;
        }

        public override void Print()
        {
            base.Print();
            Console.WriteLine("Имя водителя: " + Fio + " Тариф за км: " + FareRate);
        }

        public double CalculateFare(double distance)
        {
            return distance * FareRate;
        }
    }
}
