using CrocoManager.Core.DTOs;
using CrocoManager.Core.Interfaces;
using CrocoManager.Core.Mappers;
using CrocoManager.Core.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrocoManager.Core.Services
{
    public class ObservationService : BaseService<ObservationDto>, IObservationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _waterDataApiUrl = "https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00480&period=PT2H";
        private readonly string _weatherDataApiUrl = "https://api.weather.gov/stations/KHST/observations/latest";
        private readonly ISupabaseClientService _supabase;
        public ObservationService(ISupabaseClientService supabaseClient, HttpClient httpClient)
           : base(supabaseClient)
        {
            _httpClient = httpClient;
            _supabase = supabaseClient;
        }

        public async Task<EnvironmentalData> FetchEnvironmentalDataAsync()
        {
            try
            {
                var (waterTemp, salinity) = await FetchWaterData();

                decimal? airTemp = null;
                decimal? humidity = null;

                try
                {
                    (airTemp, humidity) = await FetchWeatherData();
                }
                catch (Exception ex)
                {
                    // For weather data, we might want to continue even if it fails, 
                    // but we should still check if it's a connection issue.
                    // If we want to strictly enforce internet for everything:
                    HandleException(ex);
                    Console.WriteLine($"Error fetching weather data: {ex.Message}");
                }

                decimal phValue = GeneratePhValue();

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
            catch (Exception ex)
            {
                HandleException(ex);
                throw; // HandleException already throws, but compiler might need this or just let it bubble
            }
        }

        private async Task<(decimal? airTemp, decimal? humidity)> FetchWeatherData()
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

            var properties = jsonDoc.RootElement.GetProperty("properties");

            decimal? airTemperature = null;
            decimal? relativeHumidity = null;

            if (properties.TryGetProperty("temperature", out var tempProp) && tempProp.TryGetProperty("value", out var tempValue) && tempValue.ValueKind == JsonValueKind.Number)
                airTemperature = tempValue.GetDecimal();

            if (properties.TryGetProperty("relativeHumidity", out var humProp) && humProp.TryGetProperty("value", out var humValue) && humValue.ValueKind == JsonValueKind.Number)
                relativeHumidity = humValue.GetDecimal();

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
        /// Generates a pseudo-random pH value based on the current time.
        /// This simulates a natural diurnal cycle (pH fluctuates with photosynthesis/respiration)
        /// and remains consistent even after app restarts.
        /// </summary>
        private static decimal GeneratePhValue()
        {
            var now = DateTime.UtcNow;

            // Diurnal cycle: pH is higher during the day (photosynthesis) and lower at night.
            // We use a sine wave with a 24h period, peaking in the late afternoon (~16:00 UTC).
            double hourFraction = now.Hour + (now.Minute / 60.0);
            double diurnalCycle = Math.Sin((hourFraction - 10.0) * Math.PI / 12.0);

            decimal basePh = 7.8m;
            decimal amplitude = 0.4m; // Fluctuation between 7.4 and 8.2

            decimal ph = basePh + (amplitude * (decimal)diurnalCycle);

            // Add deterministic jitter based on 10-minute blocks to simulate local variations.
            // Using a seed based on the date and the 10-minute block ensures consistency across restarts.
            int timeBlock = (int)(now.TimeOfDay.TotalMinutes / 10);
            int seed = now.Year + now.DayOfYear + timeBlock;
            var rnd = new Random(seed);

            decimal jitter = (decimal)(rnd.NextDouble() * 0.1 - 0.05); // +-0.05

            return Math.Round(ph + jitter, 2);
        }
    }
}
