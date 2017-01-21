namespace Seade.Core
{
    public interface IConfigurationService
    {
        T Load<T>() where T : ConfigurationBase;
        void Load(ConfigurationBase file);
    }
}