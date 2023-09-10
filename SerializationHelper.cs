using System.Reflection;
using System.Xml.Linq;

namespace CliFramework
{
    public static class SerializationHelper
    {
        public static T SerializeXElement<T>(XElement element) where T : new()
        {
            T obj = new();
            Type type = typeof(T);
            foreach (var field in type.GetFields())
            {
                XAttribute attribute = element.Attribute(field.Name);
                if (attribute != null)
                {
                    object parsedValue = ParseValue(attribute.Value, field.FieldType);
                    field.SetValue(obj, parsedValue);
                }
            }
            foreach (var property in type.GetProperties())
            {
                XAttribute attribute = element.Attribute(property.Name);
                if (attribute != null)
                {
                    object parsedValue = ParseValue(attribute.Value, property.PropertyType);
                    property.SetValue(obj, parsedValue);
                }
            }
            return obj;
        }

        private static object ParseValue(string value, Type targetType)
        {
            if (targetType == typeof(int))
            {
                if (int.TryParse(value, out int result)) return result;
                else return default(int);
            }
            else if (targetType == typeof(float))
            {
                if (float.TryParse(value, out float result)) return result;
                else return default(float);
            }
            else if (targetType == typeof(double))
            {
                if (double.TryParse(value, out double result)) return result;
                else return default(double);
            }
            else if (targetType == typeof(bool))
            {
                if (bool.TryParse(value, out bool result)) return result;
                else return default(bool);
            }
            else if (targetType == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime result)) return result;
                else return default(DateTime);
            }
            else if (targetType == typeof(Guid))
            {
                if (Guid.TryParse(value, out Guid result)) return result;
                else return default(Guid);
            }
            return value;
        }
    }
}