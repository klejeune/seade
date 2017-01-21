using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Seade;
using Seade.Test.Models;

namespace Seade.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NoModelTest()
        {
            var model = new ConfigLevel1(this.BuildConfigurationService());

            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void FilledModelTest()
        {
            var textValue = "my text value";
            var nameValue = "my name value";
            var infoValue = "my info value";
            var nameValue1 = "my name value1";
            var infoValue1 = "my info value1";
            var nameValue2 = "my name value2";
            var infoValue2 = "my info value2";
            var jsonConfig = $"{{ Text :\"{textValue}\", Date: \"2012-04-23T18:25:43.511Z\", Object: {{ Name: \"{nameValue}\", Info: \"{infoValue}\" }}, ObjectList: [{{ Name: \"{nameValue1}\", Info: \"{infoValue1}\" }},{{ Name: \"{nameValue2}\", Info: \"{infoValue2}\" }}], EnumerableList: [ \"first\", \"second\" ] }}";

            var service = this.BuildConfigurationService(new Dictionary<string, string>
            {
                {"ConfigLevel1", jsonConfig}
            });

            var model = new ConfigLevel1(service);

            Assert.IsNotNull(model);
            Assert.AreEqual(textValue, model.Text);
            Assert.AreEqual(new DateTime(2012, 4, 23, 18, 25, 43, 511), model.Date);
            Assert.AreEqual(nameValue, model.Object.Name);
            Assert.AreEqual(infoValue, model.Object.Info);
            Assert.AreEqual(nameValue1, model.ObjectList[0].Name);
            Assert.AreEqual(infoValue1, model.ObjectList[0].Info);
            Assert.AreEqual(nameValue2, model.ObjectList[1].Name);
            Assert.AreEqual(infoValue2, model.ObjectList[1].Info);
            Assert.IsNotNull(model.ReadOnlyText);
            Assert.IsNotNull(model.EnumerableList);
        }

        [TestMethod]
        public void CreateDefaultModelTest()
        {
            var files = new Dictionary<string, string>();
            var service = this.BuildConfigurationService(files);

            var model = new ConfigLevel1(service);

            Assert.IsNotNull(model);
            Assert.IsTrue(files.ContainsKey("ConfigLevel1"));
            Assert.IsNotNull(model.Object);
            Assert.IsNotNull(model.ObjectList);
            Assert.IsNotNull(model.EnumerableList);
            Assert.IsNotNull(model.Array);
            Assert.AreEqual("", model.Text);
            Assert.IsNotNull(model.ReadOnlyText);
            Assert.IsNull(model.IgnoredText);
        }

        [TestMethod]
        public void DefaultValueProviderTest()
        {
            var defaultValue = "default-value";

            var provider = new TestDefaultValueProvider(propertyInfo =>
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    return defaultValue;
                }
                return null;
            });

            var service = this.BuildConfigurationService(new Dictionary<string, string>(), provider);

            var model = new ConfigLevel1(service);

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Object);
            Assert.IsNotNull(model.ObjectList);
            Assert.IsNotNull(model.EnumerableList);
            Assert.IsNotNull(model.Array);
            Assert.AreEqual(defaultValue, model.Text);
            Assert.AreEqual(defaultValue, model.ReadOnlyText);
            Assert.IsNull(model.IgnoredText);
        }

        private IConfigurationService BuildConfigurationService(Dictionary<string, string> files = null, IDefaultValueProvider defaultValueProvider = null)
        {
            if (files == null)
            {
                files = new Dictionary<string, string>();
            }
            
            var service = new ConfigurationService(
                type => GetValueOrDefault(files, type.Name), 
                (type, value) => files[type.Name] = value,
                defaultValueProvider);

            return service;
        }

        private TValue GetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue result;

            if (!dictionary.TryGetValue(key, out result))
            {
                result = default(TValue);
            }

            return result;
        }
    }
}
