using Newtonsoft.Json;

namespace EntreLaunch.PayTabsHosted.API.Converters
{
    /// <summary>
    /// JSON Converter class for decimals.
    /// This class is used as a JSON Converter to format decimal values when converted to JSON format.
    /// </summary>
    /// <remarks>
    /// Removes fractional part from decimals if not required. Important for Japanse Yen transactions as API may throw an exception otherwise.
    /// </remarks>
    internal class DecimalJsonConverter : JsonConverter
    {
        public DecimalJsonConverter()
        {
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            decimal decimalValue = (decimal)value;
            if (decimalValue == Math.Truncate(decimalValue))
            {
                writer.WriteRawValue(JsonConvert.ToString(Convert.ToInt64(value)));
            }
            else
            {
                writer.WriteRawValue(JsonConvert.ToString(value));
            }
        }
    }
}
