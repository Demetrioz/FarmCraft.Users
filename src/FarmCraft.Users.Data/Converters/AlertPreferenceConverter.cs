using FarmCraft.Users.Data.Entities;
using Newtonsoft.Json;

namespace FarmCraft.Users.Data.Converters
{
    public class AlertPreferenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer
        )
        {
            try
            {
                string value =  reader.Value == null
                    ? ""
                    : (string)reader.Value;

                switch (value)
                {
                    case "Email":
                        return AlertPreference.Email;
                    case "Text":
                        return AlertPreference.Text;
                    default:
                        return AlertPreference.None;
                }
            }
            catch (Exception)
            {
                return AlertPreference.None;
            }
        }

        public override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer
            serializer
        )
        {
            try
            {
                AlertPreference level = value != null
                    ? (AlertPreference)value
                    : AlertPreference.None;

                switch (level)
                {
                    case AlertPreference.Email:
                        writer.WriteValue("Email");
                        break;
                    case AlertPreference.Text:
                        writer.WriteValue("Text");
                        break;
                    case AlertPreference.None:
                        writer.WriteValue("None");
                        break;
                    default:
                        writer.WriteNull();
                        break;
                }
            }
            catch (Exception)
            {
                writer.WriteNull();
            }
        }
    }
}
