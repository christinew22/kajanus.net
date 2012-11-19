namespace UsingCVDWithoutMap.Framework
{
    using System;
    using System.Collections.Generic;

    public static class NavigationContext
    {
        private static Dictionary<string, object> contexts = new Dictionary<string, object>();

        public static void SetContext(string key, object value)
        {
            if (contexts.ContainsKey(key))
            {
                throw new ArgumentException("Key already exists.");
            }

            contexts.Add(key, value);
        }

        /// <summary>
        /// Get context 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetContext<T>(string key) where T : class
        {
            if (!contexts.ContainsKey(key))
            {
                return null;
            }

            var context = contexts[key] as T;
            if (context == null)
            {
                throw new InvalidCastException("Cannot cast context to target type.");
            }
            else
            {
                contexts.Remove(key);
                return context;
            }

            return null;
        }
    }
}
