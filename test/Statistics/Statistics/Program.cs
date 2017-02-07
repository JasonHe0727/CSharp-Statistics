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


            df.AddRow("John", 27);
            df.AddRow("Michael", 98);
            df.AddRow("Rue", 39);
            df.AddRow("Peeta", 78);

            df.PrimaryKey = df["name"];


            DataFrame df2 = df[df["age"] > 40];
           // DataFrame df2 = df[df["age"] > 40 & df["name"] == "Michael"];

            Console.WriteLine(df);
            Console.WriteLine("======================");
            Console.WriteLine(df2);

          
            Console.WriteLine();
            //  df.SaveToFile("test_df.cdf", Encoding.UTF8);
            Console.WriteLine("Finished.");
        }
    }
}
