using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace string_format_performance_test
{
    class Program
    {

        const int LOOP_SIZE = 10000;
        const string firstName = "Shadman";
        const string lastName = "Kudchikar";
        const int id = 1;
        const string department = ".NET Team";

        static void PerformanceTest1()
        {
            string nameString = String.Empty;

            for (int i = 0; i < LOOP_SIZE; i++)
                nameString += String.Format("{0} {1}", firstName, lastName);
        }

        static void PerformanceTest2()
        {
            StringBuilder builder = new StringBuilder((firstName.Length + lastName.Length + 1) * LOOP_SIZE);

            for (int i = 0; i < LOOP_SIZE; i++)
                builder.Append(String.Format("{0} {1}", firstName, lastName));

            string nameString = builder.ToString();
        }

        static void PerformanceTest3()
        {
            StringBuilder builder = new StringBuilder((firstName.Length + lastName.Length + 1) * LOOP_SIZE);

            for (int i = 0; i < LOOP_SIZE; i++)
                builder.AppendFormat("{0} {1}", firstName, lastName);

            string nameString = builder.ToString();
        }

        static void PerformanceTest4()
        {
            string htmlString;
            for (int i = 0; i < LOOP_SIZE; i++)
                htmlString = "<td>" + String.Format("{0} {1}", firstName, lastName) + "</td><td>"
                  + String.Format("{0}, {1}", id, department) + "</td>";
        }
        static void PerformanceTest5()
        {
            StringBuilder builder = new StringBuilder(256);

            string htmlString;
            for (int i = 0; i < LOOP_SIZE; i++)
            {
                builder.Append("<td>");
                builder.AppendFormat("{0} {1}", firstName, lastName);
                builder.Append("</td><td>");
                builder.AppendFormat("{0}, {1}", id, department);
                builder.Append("</td>");
                htmlString = builder.ToString();
                builder.Length = 0;
            }
        }

        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            PerformanceTest1();
            Console.WriteLine($"PerformanceTest1: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Restart();
            PerformanceTest2();
            Console.WriteLine($"PerformanceTest2: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Restart();
            PerformanceTest3();
            Console.WriteLine($"PerformanceTest3: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Restart();
            PerformanceTest4();
            Console.WriteLine($"PerformanceTest4: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Restart();
            PerformanceTest5();
            Console.WriteLine($"PerformanceTest5: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Stop();
            Console.ReadLine();
        }
    }
}
