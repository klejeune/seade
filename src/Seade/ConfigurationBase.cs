namespace Seade
{
    public abstract class ConfigurationBase
    {
        protected ConfigurationBase(IConfigurationService configurationService)
        {
            configurationService.Load(this);
        }
    }
}