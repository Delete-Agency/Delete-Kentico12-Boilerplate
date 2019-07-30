using System;

namespace DeleteBoilerplate.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public StringValueAttribute(string value, string key) : this(value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            Key = key;
        }

        public string Key { get; }

        public string Value { get; }
    }
}
