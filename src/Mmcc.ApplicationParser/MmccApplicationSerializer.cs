using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mmcc.ApplicationParser
{
    /// <summary>
    /// Provides functionality to serialize objects to MMCC Application format and to deserialize MMCC Application format into objects.
    ///
    /// MMCC Application format is a simple key-value pair format that supports only basic value types and does not support nesting and nulls.
    /// </summary>
    public class MmccApplicationSerializer
    {
        /// <summary>
        /// Property types supported by the MMCC Application format.
        ///
        /// Supported types:
        /// <see cref="bool"/>
        /// <see cref="byte"/>
        /// <see cref="sbyte"/>
        /// <see cref="char"/>
        /// <see cref="decimal"/>
        /// <see cref="double"/>
        /// <see cref="int"/>
        /// <see cref="uint"/>
        /// <see cref="long"/>
        /// <see cref="ulong"/>
        /// <see cref="short"/>
        /// <see cref="ushort"/>
        /// </summary>
        private readonly List<Type> _supportedPropertyTypes = new List<Type>
        {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort)
        };

        /// <summary>
        /// Sets whether property names should be matched in a case-insensitive fashion.
        /// </summary>
        private readonly bool _propertyNamesCaseInsensitive;
        
        /// <summary>
        /// Instantiates the MmccApplicationSerializer class.
        /// </summary>
        /// <param name="propertyNamesCaseInsensitive">Sets whether property names should be matched in a case-insensitive fashion.</param>
        public MmccApplicationSerializer(bool propertyNamesCaseInsensitive = false)
        {
            _propertyNamesCaseInsensitive = propertyNamesCaseInsensitive;
        }

        /// <summary>
        /// Parses the string representing a MMCC Application format object into an instance of the type specified by a generic type parameter.
        /// </summary>
        /// <param name="applicationString">String in MMCC Application format to be deserialized into .NET object</param>
        /// <typeparam name="T">The target type</typeparam>
        /// <returns>A T representation of the MMCC Application format object</returns>
        /// <exception cref="NoNullOrWhitespaceApplicationStringException">MMCC Application string can not be null, empty or whitespace-only</exception>
        /// <exception cref="InvalidPairsException">Could not parse the string into valid key-value pairs</exception> 
        /// <exception cref="UnsupportedTypeException">Property type is unsupported by MMCC Application format</exception>
        /// <exception cref="InvalidCastException">Could not cast a value into its property type</exception>
        public T Deserialize<T>(string applicationString) where T : new()
        {
            if (string.IsNullOrWhiteSpace(applicationString))
            {
                throw new NoNullOrWhitespaceApplicationStringException("MMCC Application string can not be null, empty or whitespace-only.");
            }
            
            var deserializedObj = new T();
            
            using var stringReader = new StringReader(applicationString);
            string line;
            while ((line = stringReader.ReadLine()) is not null)
            {
                string propertyName;
                string value;
                try
                {
                    propertyName = line.Substring(0, line.IndexOf(':'));
                    value = line.Substring(line.IndexOf(':') + 1).Trim();
                }
                catch(Exception e)
                {
                    throw new InvalidPairsException("Could not parse the string into valid key-value pairs.", e);
                }
                
                var matchedProperty = GetMatchedProperty<T>(propertyName);
                var matchedPropertyType = matchedProperty.PropertyType;
                
                if (!IsSupportedType(matchedPropertyType))
                {
                    throw new UnsupportedTypeException($"Property \"{matchedProperty}\" is a type that is unsupported by MMCC Application format.");
                }
                if (matchedPropertyType != typeof(string))
                {
                    try
                    {
                        var matchedConverter = TypeDescriptor.GetConverter(matchedPropertyType);
                        var convertedObj = matchedConverter.ConvertFromString(value);
                        matchedProperty.SetValue(deserializedObj, convertedObj);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidCastException($"Could not cast value \"{value}\" to type \"{matchedPropertyType}\"", e);                        
                    }
                }
                else
                {
                    matchedProperty.SetValue(deserializedObj, value);
                }
            }

            return deserializedObj;
        }
        
        /// <summary>
        /// Converts a .NET object of a type specified by a generic type parameter into a MMCC Application format string.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <typeparam name="T">Type of the object to serialize</typeparam>
        /// <returns>A MMCC application format string representation of the object</returns>
        /// <exception cref="NoPropertiesFoundException">Could not find any properties for type T</exception>
        /// <exception cref="UnsupportedTypeException">Property type is unsupported by MMCC Application format</exception>
        public string Serialize<T>(T obj)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (!properties.Any())
            {
                throw new NoPropertiesFoundException($"Could not find any properties for type {nameof(T)}");
            }
            
            var stringBuilder = new StringBuilder();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                
                if (!IsSupportedType(propertyType))
                {
                    throw new UnsupportedTypeException($"Property \"{propertyType}\" is a type that is unsupported by MMCC Application format.");
                }
                
                var value = property.GetValue(obj, null);

                if (value is not null)
                {
                    stringBuilder.AppendLine($"{property.Name}: {value}");
                }
            }

            return stringBuilder.ToString();
        }
        
        /// <summary>
        /// Matches a property to a MMCC Application format key.
        /// </summary>
        /// <param name="key">Key to match to a property</param>
        /// <typeparam name="T">Parent type that contains properties to match</typeparam>
        /// <returns>Matched property</returns>
        /// <exception cref="MissingPropertyException">A matching property could not be found for a given key</exception>
        private PropertyInfo GetMatchedProperty<T>(string key)
        {
            var matchedProperty = _propertyNamesCaseInsensitive == false
                ? typeof(T).GetProperty(key,
                    BindingFlags.Public | BindingFlags.Instance)
                : typeof(T).GetProperty(key,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            if (matchedProperty is null)
            {
                throw new MissingPropertyException($"Could not find a property corresponding to key \"{key}\"");
            }

            return matchedProperty;
        }
        
        /// <summary>
        /// Checks if a type is supported by MMCC Application format.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>Whether the given type is supported</returns>
        private bool IsSupportedType(Type type)
        {
            return _supportedPropertyTypes.Contains(type);
        }
    }
}