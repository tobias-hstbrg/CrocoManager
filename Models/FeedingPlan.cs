using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CrocoManager.Models
{
    [Table("feeding_plan")]
    public class FeedingPlan : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("food_type")]
        public string FoodType { get; set; } = string.Empty;

        [Column("amount_kg")]
        public double AmountKg { get; set; }

        [Column("frequency_per_week")]
        public int FrequencyPerWeek { get; set; }

        [Column("weekdays")]
        [JsonConverter(typeof(WeekdayListConverter))]
        public List<Weekday> Weekdays { get; set; } = [];

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
    public enum Weekday
    {
        Montag,
        Dienstag,
        Mittwoch,
        Donnerstag,
        Freitag,
        Samstag,
        Sonntag
    }

    /// <summary>
    /// Reads json array of strings and tries to convert its contents to Weekday enum values.
    /// </summary>
    public class WeekdayListConverter : JsonConverter<List<Weekday>>
    {
        public override List<Weekday> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected start of array");
            }

            var list = new List<Weekday>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return list;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    string? value = reader.GetString();
                    if (value != null && Enum.TryParse<Weekday>(value, true, out var weekday))
                    {
                        list.Add(weekday);
                    }
                }
            }

            throw new JsonException("Expected end of array");
        }

        public override void Write(Utf8JsonWriter writer, List<Weekday> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var weekday in value)
            {
                writer.WriteStringValue(weekday.ToString());
            }

            writer.WriteEndArray();
        }
    }
}
