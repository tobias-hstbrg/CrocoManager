using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrocoManager.DTOs;
using CrocoManager.Models;
using System.Text.Json;

namespace CrocoManager.Services
{
    public class ObservationService : BaseService<ObservationDto>
    {
        private readonly HttpClient _httpClient;
        private readonly string _waterDataApiUrl = "https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00095&period=PT2H";

        public ObservationService(SupabaseClientService supabaseClient, HttpClient httpClient)
           : base(supabaseClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EnvironmentalData> FetchEnvironmentalDataAsync()
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

                if(!measurements.EnumerateArray().Any())
                    continue;

                var latest = measurements.EnumerateArray().Last();
                var valueStr = latest.GetProperty("value").GetString();

                decimal.TryParse(valueStr, out decimal value);

                switch(variableCode)
                {
                    case "00010": // Water Temperature
                        waterTemp = value;
                        break;
                    case "00095": // salinity in micro siemens per centimeter
                        salinity = value;
                        break;
                }
            }

            if ((waterTemp == null || salinity == null))
                throw new InvalidOperationException("Required water values not found in API response.");

            var salinityPpt = ConvertMicroSiemensToPpt(salinity.Value);

            return new EnvironmentalData(
                measurementDate: DateOnly.FromDateTime(DateTime.UtcNow),
                measurementTime: DateTime.UtcNow.TimeOfDay,
                airTemperatureCelsius: 0,
                humidityPercent: 0,
                waterTemperatureCelsius: waterTemp.Value,
                phValue: 0,
                salinityPpt: salinityPpt
            );
        }

        /// <summary>
        /// Converts a value from microsiemens (μS/cm) to parts per thousand (ppt).
        /// </summary>
        /// <remarks>This conversion uses a simple linear factor and does not account for temperature or
        /// other environmental variables that may affect conductivity measurements.</remarks>
        /// <param name="microSiemens">The electrical conductivity value in microsiemens (μS/cm) to convert. Must be greater than or equal to zero.</param>
        /// <returns>The equivalent value in parts per thousand (ppt).</returns>
        private decimal ConvertMicroSiemensToPpt(decimal microSiemens)
        {
            return microSiemens * 0.001m;
        }
    }
}
