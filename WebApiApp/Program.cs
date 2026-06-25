using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApiApp
{
    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class WeatherAlert
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("severity")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AlertSeverity Severity { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("--- SYSTEM MONITOR ---");
            Console.WriteLine("Fetching logs...");

            string url = "https://raw.githubusercontent.com/polygonstew/mock-api/main/alerts.json";

            try
            {
                string jsonResponse = await client.GetStringAsync(url);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<WeatherAlert> alerts = JsonSerializer.Deserialize<List<WeatherAlert>>(jsonResponse, options);

                if (alerts != null && alerts.Count > 0)
                {
                    Console.WriteLine($"Found {alerts.Count} records.\n");
                    
                    Console.WriteLine("Location   | Severity   | Timestamp            | Message");
                    Console.WriteLine("----------------------------------------------------------------");
                    
                    foreach (var alert in alerts)
                    {
                        if (alert.Severity == AlertSeverity.Critical)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else if (alert.Severity == AlertSeverity.High)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }

                        Console.WriteLine($"{alert.Location,-10} | {alert.Severity,-10} | {alert.Timestamp.ToLocalTime(),-20} | {alert.Message}");
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("No data parsed.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nDone. Press any key to close.");
            Console.ReadKey();
        }
    }
}