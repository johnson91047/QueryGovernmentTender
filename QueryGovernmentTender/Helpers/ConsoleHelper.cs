using System;

namespace QueryGovernmentTender.Helpers
{
    public static class ConsoleHelper
    {
        public static void PrintDivider()
        {
            Print("===================================================");
        }

        public static void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
