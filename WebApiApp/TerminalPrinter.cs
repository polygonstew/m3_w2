using System;
using System.Collections.Generic;

namespace WebApiApp
{
    public static class TerminalPrinter
    {
        public static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("        SECWATCH WEATHER ALERT TERMINAL          ");
            Console.WriteLine("==================================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        public static void PrintTable(List<WeatherAlert> alerts)
        {
            Console.WriteLine("{0,-12} | {1,-10} | {2,-22} | {3}", "Location", "Severity", "Timestamp", "Message");
            Console.WriteLine(new string('-', 80));

            foreach (var alert in alerts)
            {
                switch (alert.Severity)
                {
                    case AlertSeverity.Critical:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case AlertSeverity.High:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case AlertSeverity.Medium:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }

                Console.WriteLine("{0,-12} | {1,-10} | {2,-22:yyyy-MM-dd HH:mm:ss} | {3}", 
                    alert.Location, 
                    alert.Severity, 
                    alert.Timestamp.ToLocalTime(), 
                    alert.Message);
            }
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERROR]: {message}");
            Console.ResetColor();
        }
    }
}