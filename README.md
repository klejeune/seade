# Seade
A C# configuration management system. Code-first, lightweight.

https://www.nuget.org/packages/Seade-Config 

## Why another configuration management system?
.Net settings are not good enough:
* they are written in XML
* they usually are contained in a web.config or app.config, mixing framework, technical and business values

## What is Seade?
Seade can help you define better configuration files/classes:
* define your settings in a C# class
* your configuration file(s) are written in json
* you can split your configuration in seperate files/directories

Define your settings in a C# class, make it inherit the ConfigurationBase class, define where you want to read your settings, and you're set.

## How does it work?
Here is an example.

Install the Seade Nuget package: 
```
Install-Package Seade-Config 
```

Define your settings class:
```C#
public class MyConfiguration : ConfigurationBase
{
    public MyConfiguration(IConfigurationService configurationService) : base(configurationService) {}

    public string FirstSetting { get; set; }
    public List<string> SettingList { get; set; }
}
```

At the startup of your app or website, create a ConfigurationService instance :
```C#
var service = new ConfigurationService(type => File.ReadAllText($@"conf\{type.Name}.json"));
```

You can instantiate you settings class by calling:
```C#
var myConfiguration = new MyConfiguration(service);
```
The values will be read from your json file during the MyConfiguration() constructor execution.

## Features
### Inversion of control
This implementation is compatible with most inversion of control dependency containers, like StructureMap, Unity...
In most cases, you may define the configuration instance at startup and always use the same instance.

### Multiple files
You can split your configuration in several classes/files to make it more readable. The ConfigurationService constructor parameter allows you to choose
	where your fils are stored. You can event reproduce the same direcotry structure as your code by using the namespace values.

### JSON configuration file template
An second optional parameter can be used to create a default configuration file with the right structure:
```C#
var service = new ConfigurationService(
	type => File.ReadAllText($@"conf\{type.Name}.json"),
	(type, content) => File.WriteAllText($@"conf\{type.Name}.json", content));
```

### Non configurable settings
You can define non configurable settings by
	providing a constant value instead of a setter. The json file won't contain the setting, but you will be able to use it in your program:
```C#
    public string ReadOnlySetting => "Read only value";
```

### Read only properties
Your configuration file properties must be writeable (ie. have a "set;") in order to be used in the json file. If you want to provide read only configuration 
	to your other services, you can:
* make the setters private (bad because it won't be easily useable in unit tests)
* make the setters protected (better because your unit tests will be able to set values in a subclass)
* let the setters public, but provide an read only interface (without the setters) to your other services

### Custom default values
If a json setting is null or absent, the associated property will have a default value.

Default values for some types are provided:
- string ("")
- array (0-sized array)
- IEnumerable<> (empty array)
- IDictionary<,> (emtpy dictionary)
- class types with a parameterless constructor

If extra types need to have default values, or if the provided ones must be changed, you can inherit from DefaultValueProvider:
```C#
public class CustomDefaultValueProvider : DefaultValueProvider
{
    public override object GetDefaultValue(PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType == typeof(string))
        {
            return "my default string;
        }
        else
        {
            return base.GetDefaultValue(propertyInfo);
        }
    }
}

var service = new ConfigurationService(type => File.ReadAllText($@"conf\{type.Name}.json"), new CustomDefaultValueProvider());
```