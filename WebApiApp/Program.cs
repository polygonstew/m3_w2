using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApiApp
{
    // 1. THE ENUM
    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    // 2. THE DATA MODEL
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

    // 3. THE MAIN PROGRAM
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            TerminalPrinter.PrintHeader();
            Console.WriteLine("Fetching real-time logs...");

            string url = "https://raw.githubusercontent.com/polygonstew/m3_w2/main/alerts.json";

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
                    Console.WriteLine($"Successfully retrieved {alerts.Count} records.\n");
                    TerminalPrinter.PrintTable(alerts);
                }
                else
                {
                    Console.WriteLine("No active alerts found.");
                }
            }
            catch (Exception ex)
            {
                TerminalPrinter.PrintError(ex.Message);
            }

            Console.WriteLine("\nPress any key to exit terminal sequence...");
            Console.ReadKey();
        }
    }
}