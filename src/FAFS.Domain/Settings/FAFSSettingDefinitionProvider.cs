using Volo.Abp.Settings;

namespace FAFS.Settings;

public class FAFSSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FAFSSettings.MySetting1));
    }
}
