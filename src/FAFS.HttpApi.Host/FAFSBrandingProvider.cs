using Microsoft.Extensions.Localization;
using FAFS.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace FAFS;

[Dependency(ReplaceServices = true)]
public class FAFSBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FAFSResource> _localizer;

    public FAFSBrandingProvider(IStringLocalizer<FAFSResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
