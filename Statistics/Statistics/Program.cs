using System;
using Statistics.DataContainers;

namespace Statistics
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            DataFrame df = new DataFrame();
            df.AddColumn("name", typeof(string));
            df.AddColumn("age", typeof(int));

            Random rnd = new Random();
            string[] names = { "John", "Michael", "Joker", "Batman", "Rue", "Peeta" };
            foreach (var name in names)
            {
                df.AddRow(name, rnd.Next(1, 50));
            }
            Console.WriteLine(df);
            Console.WriteLine("=========================");
            Console.WriteLine(df[2, 0]);
            Console.WriteLine(df[2, 1]);
        }
    }
}
