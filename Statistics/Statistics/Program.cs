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


            var df2 = df[df["age"] > 40 & df["name"] == "Michael"];

            Console.WriteLine(df);
            Console.WriteLine("======================");
            Console.WriteLine(df2);
            /*Console.WriteLine(df.IndexOfColumn("name"));
            Console.WriteLine(df.IndexOfKey("Rue"));
            Console.WriteLine(string.Join(", ", df["age"].As<int>()));
            Console.WriteLine(df);

            var a = df.MapToClass(
                        row => new MyClass(){ name = (string)row["name"], age = (int)row["age"] });
            
            foreach (var item in a)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();*/
            // Console.WriteLine(df.Subset(row => (int)row["age"] > 40));
            //Console.WriteLine(df.Subset(row => row.RowIndex < 2));
            Console.WriteLine();

            //  df.SaveToFile("test_df.cdf", Encoding.UTF8);
            Console.WriteLine("Finished.");
        }
    }

    class MyClass
    {
        public string name;
        public int age;

        public override string ToString()
        {
            return string.Format("Person: {0}, {1}", name, age);
        }
    }
}
