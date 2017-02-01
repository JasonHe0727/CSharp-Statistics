using System;
using Statistics.DataContainers;
using System.Text;

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
            // df[0, 1] = null;
            Console.WriteLine(df);
            Console.WriteLine(df.GetRow(0)[0]);
            //  df.SaveToFile("test_df.cdf", Encoding.UTF8);
            Console.WriteLine("Finished.");
        }
    }
}
