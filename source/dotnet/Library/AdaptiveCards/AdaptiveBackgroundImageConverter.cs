using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptiveCards
{
    public class AdaptiveBackgroundImageConverter : JsonConverter, ILogWarnings
    {
        public List<AdaptiveWarning> Warnings { get; set; } = new List<AdaptiveWarning>();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AdaptiveBackgroundImage bi = (AdaptiveBackgroundImage) value;
            if (bi.HasDefaultValues())
            {
                writer.WriteValue(bi.BackgroundImageString);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken backgroundImageJSON = JToken.Load(reader);
            // Handle BackgroundImage as a string (BackCompat)
            if (backgroundImageJSON.Type == JTokenType.String)
            {
                //card.BackgroundImage = new AdaptiveBackgroundImage(jObject.Value<string>("backgroundImage"));
                return new AdaptiveBackgroundImage(backgroundImageJSON.Value<string>());
            }
            // backgroundImage is an object (Modern)
            else if (backgroundImageJSON.Type == JTokenType.Object)
            {
                //card.BackgroundImage = (AdaptiveBackgroundImage)Activator.CreateInstance(typeof(AdaptiveBackgroundImage));
                //serializer.Populate(backgroundImageJSON.CreateReader(), card.BackgroundImage);
                return backgroundImageJSON.ToObject<AdaptiveBackgroundImage>();
            }
            else
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            // string --> BackCompat
            // AdaptiveBackgroundImage --> Modern
            return objectType == typeof(string) || objectType == typeof(AdaptiveBackgroundImage);
        }
    }
}