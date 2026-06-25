using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApiConsoleApp
{
    // Requirement: Enum to handle constant string/integer categories from an API
    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    // Data model for deserialization
    public class WeatherAlert
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        // Requirement: Mapping API string constants to a strongly-typed Enum
        [JsonPropertyName("severity")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AlertSeverity Severity { get; set; }

        // Requirement: Utilizing DateTime for API time values
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    class Program
    {
        // HttpClient is intended to be instantiated once and reused throughout the application life
        private static readonly HttpClient client = new HttpClient();

        // Using async Task Main to support asynchronous execution flow
        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================");
            Console.WriteLine("        SECWATCH WEATHER ALERT TERMINAL          ");
            Console.WriteLine("==================================================\n");
            Console.ResetColor();

            // Utilizing a reliable public mock API endpoint that returns a JSON array matching our schema
            string url = "https://raw.githubusercontent.com/polygonstew/m3_w2/main/alerts.json";

            try
            {
                Console.WriteLine("Fetching real-time alert logs via HttpClient...");
                
                // Requirement: Use HttpClient along with async and await
                string jsonResponse = await client.GetStringAsync(url);

                // Configure serialization options to match case-insensitive properties if needed
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Requirement: Use List<T> to manage arrays of data returned by the API
                List<WeatherAlert>? alerts = JsonSerializer.Deserialize<List<WeatherAlert>>(jsonResponse, options);

                if (alerts != null && alerts.Count > 0)
                {
                    Console.WriteLine($"\nSuccessfully retrieved and deserialized {alerts.Count} records.\n");
                    PrintAlertTable(alerts);
                }
                else
                {
                    Console.WriteLine("No active alerts found or failed to parse data.");
                }
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[HTTP Error]: Connection to the API failed.");
                Console.WriteLine($"Details: {e.Message}");
                Console.ResetColor();
            }
            catch (JsonException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Deserialization Error]: Failed to map incoming JSON data.");
                Console.WriteLine($"Details: {e.Message}");
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Unexpected Error]: {e.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nPress any key to exit terminal sequence...");
            Console.ReadKey();
        }

        private static void PrintAlertTable(List<WeatherAlert> alerts)
        {
            // Format output headers
            Console.WriteLine(string.Format("{0,-12} | {1,-10} | {2,-22} | {3}", "Location", "Severity", "Timestamp (Local)", "Message"));
            Console.WriteLine(new string('-', 85));

            foreach (var alert in alerts)
            {
                // Set text color dynamically based on our Enum values
                switch (alert.Severity)
                {
                    case AlertSeverity.Critical:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case AlertSeverity.High:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case AlertSeverity.Medium:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }

                // Requirement display: Formatting DateTime directly within the output format
                Console.WriteLine(string.Format("{0,-12} | {1,-10} | {2,-22:yyyy-MM-dd HH:mm:ss} | {3}", 
                    alert.Location, 
                    alert.Severity, 
                    alert.Timestamp.ToLocalTime(), 
                    alert.Message));
            }
            Console.ResetColor();
        }
    }
}