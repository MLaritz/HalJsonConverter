using System;
using System.Collections;
using System.Linq;
using Hal.Json.Attributes;
using Hal.Json.Extensions;
using Hal.Json.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hal.Json
{
    public class HalJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = JToken.FromObject(value);
            var o = (JObject)t;

            CamelCasePropertyNames(value, o);

            AddLinksProperty(o, value);

            var p = value.GetType().GetProperties();

            if (p.Any(x => x.GetCustomAttributes(typeof(EmbeddedAttribute), true).Length > 0))
            {

                foreach (var property in p.Where(x => x.GetCustomAttributes(typeof(EmbeddedAttribute), true).Length > 0))
                {
                    var propertyInfo = value.GetType().GetProperty(property.Name);
                    var v = propertyInfo.GetValue(value, null);
                    if (v as IEnumerable != null)
                    {
                        var _embedded = new JArray();
                        foreach (var j in v as IEnumerable)
                        {
                            _embedded.Add(GetEmbeddedObject(j));
                        }
                        var e = new JObject(new JProperty(property.Name.FirstLetterToLower(), _embedded));
                        o.Add("_embedded", e);
                    }
                    else
                    {
                        o.Add("_embedded",
                            new JObject(new JProperty(property.Name.FirstLetterToLower(), GetEmbeddedObject(v))));
                    }
                    o.Property(property.Name.FirstLetterToLower()).Remove();

                }
            }

            t.WriteTo(writer);

        }

        private JObject GetEmbeddedObject(object v)
        {
            var et = JToken.FromObject(v);
            var eo = (JObject)et;

            CamelCasePropertyNames(v, eo);

            AddLinksProperty(eo, v);
            return eo;
        }

        private static void CamelCasePropertyNames(object value, JObject o)
        {
            foreach (var prop in value.GetType().GetProperties())
            {
                if (value.GetType().GetProperty(prop.Name).GetValue(value, null) != null)
                {
                    o.Add(prop.Name.FirstLetterToLower(), o.Property(prop.Name).Value);
                }

                o.Property(prop.Name).Remove();
            }
        }

        private void AddLinksProperty(JObject o, object v)
        {
            var halObject = v as IHalModel;
            if (halObject != null && halObject.Links.Any())
            {
                var _links = new JObject();
                foreach (var link in halObject.Links)
                {
                    _links.Add(new JProperty(link.Rel, new JObject(new JProperty("href", link.Href))));
                }
                o.Property("links").Remove();

                o.AddFirst(new JProperty("_links", _links));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetInterfaces().Any(x => x == typeof(IHalModel));
        }

        public override bool CanRead
        {
            get { return false; }
        }
    }
}