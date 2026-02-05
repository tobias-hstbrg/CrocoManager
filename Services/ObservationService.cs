using CrocoManager.DTOs;
using CrocoManager.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrocoManager.Services
{
    public class ObservationService : BaseService<ObservationDto>
    {
        private readonly HttpClient _httpClient;
        private readonly string _waterDataApiUrl = "https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00480&period=PT2H";
        private readonly string _weatherDataApiUrl = "https://api.weather.gov/stations/KTMB/observations/latest";
        private static int _phCallCount = 0;
        public ObservationService(SupabaseClientService supabaseClient, HttpClient httpClient)
           : base(supabaseClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EnvironmentalData> FetchEnvironmentalDataAsync()
        {
            var (waterTemp, salinity) = await FetchWaterData();

            var (airTemp, humidity) = await FetchWeatherData();

            decimal phValue = ((decimal)GeneratePhValues());

            return new EnvironmentalData(
                measurementDate: DateOnly.FromDateTime(DateTime.UtcNow),
                measurementTime: DateTime.UtcNow.TimeOfDay,
                airTemperatureCelsius: airTemp,
                humidityPercent: humidity,
                waterTemperatureCelsius: waterTemp,
                phValue: phValue,
                salinityPpt: salinity
            );
        }

        private async Task<(decimal, decimal)> FetchWeatherData()
        {
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "CrocoManager/1.0 (school project)"
            );

            _httpClient.DefaultRequestHeaders.Add(
                "Accept",
                "application/geo+json, application/json"
            );

            var response = await _httpClient.GetAsync(_weatherDataApiUrl);
            response.EnsureSuccessStatusCode();

            using var jsonStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(jsonStream);

            var root = jsonDoc.RootElement;

            var properties = root.GetProperty("properties");

            decimal airTemperature = properties.GetProperty("temperature").GetProperty("value").GetDecimal();
            decimal relativeHumidity = properties.GetProperty("relativeHumidity").GetProperty("value").GetDecimal();

            return (airTemperature, relativeHumidity);
        }

        private async Task<(decimal, decimal)> FetchWaterData()
        {
            var response = await _httpClient.GetAsync(_waterDataApiUrl);
            response.EnsureSuccessStatusCode();

            using var jsonStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(jsonStream);

            var root = jsonDoc.RootElement;

            var timeSeries = root.GetProperty("value").GetProperty("timeSeries");

            decimal? waterTemp = null;
            decimal? salinity = null;

            foreach (var series in timeSeries.EnumerateArray())
            {
                var variableCode = series.GetProperty("variable").GetProperty("variableCode")[0].GetProperty("value").GetString();
                var measurements = series.GetProperty("values")[0].GetProperty("value");

                if (!measurements.EnumerateArray().Any())
                    continue;

                var latest = measurements.EnumerateArray().Last();
                var valueStr = latest.GetProperty("value").GetString();

                decimal.TryParse(valueStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value);

                switch (variableCode)
                {
                    case "00010": // Water Temperature
                        waterTemp = value;
                        break;
                    case "00480": // salinity in parts per thousand
                        salinity = value;
                        break;
                }
            }

            if ((waterTemp == null || salinity == null))
                throw new InvalidOperationException("Required water values not found in API response.");

            return (waterTemp.Value, salinity.Value);
        }

        /// <summary>
        /// Converts a value from microsiemens (μS/cm) to parts per thousand (ppt).
        /// </summary>
        /// <remarks>This conversion uses a simple linear factor and does not account for temperature or
        /// other environmental variables that may affect conductivity measurements.</remarks>
        /// <param name="microSiemens">The electrical conductivity value in microsiemens (μS/cm) to convert. Must be greater than or equal to zero.</param>
        /// <returns>The equivalent value in parts per thousand (ppt).</returns>
        static private decimal ConvertMicroSiemensToPpt(decimal microSiemens)
        {
            return Math.Round(microSiemens * 0.001m, 1);
        }

        /// <summary>
        /// Generates a pseudo-random pH value that simulates natural fluctuations over time.
        /// <remarks>
        /// The value is influenced by the number of times this method has been called, creating a pattern of gradual increases and decreases to mimic real-world environmental changes.
        /// The base pH value starts around 7.6 and can vary up to 0.6 in either direction, with additional micro-fluctuations that create a more dynamic and realistic output.
        /// </remarks>
        /// </summary>
        /// <returns>Random pH value</returns>
        private static double GeneratePhValues()
        {
            _phCallCount++;

            int bucket = _phCallCount / 10;
            int hash = bucket.GetHashCode();
            double baseVariation = (Math.Abs(hash) % 100) / 100.00;
            double basePh = 7.6 + (baseVariation * 0.6);

            // Create pseudo-random but deterministic zigzag
            int microStep = _phCallCount % 10;
            int seed = bucket * 1000 + microStep;
            Random stepRnd = new Random(seed);

            // Each step randomly goes -0.02, 0, or +0.02
            double microChange = (stepRnd.Next(3) - 1) * 0.02;

            double ph = basePh + microChange;
            return Math.Round(Math.Max(7.0, Math.Min(8.4, ph)), 2);
        }
    }
}
