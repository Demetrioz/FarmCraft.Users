using FarmCraft.Users.Data.Converters;
using Newtonsoft.Json;

namespace FarmCraft.Users.Data.Entities
{
    public enum AlertPreference
    {
        Email,
        Text,
        None
    }

    public class Preferences
    {
        [JsonConverter(typeof(AlertPreferenceConverter))]
        public AlertPreference AlertPreference { get; set; }
    }
}
