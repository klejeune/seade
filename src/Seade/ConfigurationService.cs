using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Seade
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly Func<Type, string> reader;
        private readonly Action<Type, string> writer;
        private readonly IDefaultValueProvider defaultValueProvider;

        public ConfigurationService(Func<Type, string> reader) : this(reader, null, null)
        {
        }

        public ConfigurationService(Func<Type, string> reader, Action<Type, string> writer) : this(reader, writer, null)
        {
        }

        public ConfigurationService(Func<Type, string> reader, IDefaultValueProvider defaultValueProvider) : this(reader, null, defaultValueProvider)
        {
        }

        public ConfigurationService(Func<Type, string> reader, Action<Type, string> writer, IDefaultValueProvider defaultValueProvider)
        {
            this.reader = reader;
            this.writer = writer;

            this.defaultValueProvider = defaultValueProvider ?? new DefaultValueProvider();
        }

        public T Load<T>() where T : ConfigurationBase
        {
            var stringConfig = this.LoadString<T>();
            T config;

            if (!string.IsNullOrWhiteSpace(stringConfig))
            {
                config = JsonConvert.DeserializeObject<T>(stringConfig);
                this.PopulateDefault(config);
            }
            else
            {
                config = Activator.CreateInstance<T>();
                this.PopulateDefault(config);
                this.Save(config);
            }

            return config;
        }

        public void Load(ConfigurationBase file)
        {
            var config = this.LoadString(file.GetType());

            if (!string.IsNullOrWhiteSpace(config))
            {
                JsonConvert.PopulateObject(config, file);
                this.PopulateDefault(file);
            }
            else
            {
                this.PopulateDefault(file);
                this.Save(file);
            }
        }

        public void Save(ConfigurationBase file)
        {
            this.writer?.Invoke(file.GetType(), JsonConvert.SerializeObject(file, Formatting.Indented));
        }

        private T CreateDefault<T>() where T : ConfigurationBase
        {
            var d = Activator.CreateInstance<T>();
            
            this.PopulateDefault(d);

            return d;
        }

        private void PopulateDefault(object file)
        {
            foreach (var propertyInfo in file.GetType().GetRuntimeProperties())
            {
                object value = null;

                if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.GetValue(file) == null )
                {
                    value = this.defaultValueProvider.GetDefaultValue(propertyInfo);
                }

                if (value != null)
                {
                    this.PopulateDefault(value);
                    propertyInfo.SetValue(file, value);
                }
            }
        }

        private string LoadString<T>()
        {
            return this.LoadString(typeof(T));
        }

        private string LoadString(Type type)
        {
            return reader(type);
        }

        private string FindFile(Type type)
        {
            return type.Name + ".json";
        }
    }
}
