using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace BOTManager.Entities
{
    public class PropertyCollection : List<KeyValuePair<string, string>>
    {
        public PropertyCollection()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public PropertyCollection(List<KeyValuePair<string, string>> collection)
            : base(collection)
        {

        }

        /// <summary>
        /// indexer property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public object this[string keyName]
        {
            get
            {
                if (this.Exists(y => string.Equals(y.Key, keyName, StringComparison.OrdinalIgnoreCase)))
                    return this.First(y => string.Equals(y.Key, keyName, StringComparison.OrdinalIgnoreCase)).Value;
                throw new KeyNotFoundException("Key not found in collection");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string keyName, T defaultValue)
        {
            var key = this.FirstOrDefault(y => string.Equals(y.Key, keyName, StringComparison.OrdinalIgnoreCase));
            var exists = this.Exists(y => string.Equals(y.Key, keyName, StringComparison.OrdinalIgnoreCase));
            if (!exists)
                return defaultValue;

            object keyValue = key.Value;

            if (typeof(T) == typeof(bool))
            {
                keyValue = Convert.ToBoolean(!string.IsNullOrWhiteSpace(key.Value) && (key.Value != "0" || key.Value == "true" ));
            }
            return (T)Convert.ChangeType(keyValue, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public T GetValue<T>(string keyName)
        {
            var key = this.FirstOrDefault(y => string.Equals(y.Key, keyName, StringComparison.OrdinalIgnoreCase));
            var exists = this.Exists(y => string.Equals(y.Key, keyName, StringComparison.OrdinalIgnoreCase));
            if (!exists)
                throw new KeyNotFoundException("Key not found");
            object keyValue = key.Value;
            
            if (typeof(T) == typeof(bool))
            {
                keyValue = Convert.ToBoolean(!string.IsNullOrWhiteSpace(key.Value) && key.Value != "0");
            }
            
            return (T)Convert.ChangeType(keyValue, typeof(T));
        }


    }
}
