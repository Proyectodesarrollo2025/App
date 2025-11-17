using FAFS.Localization;
using Volo.Abp.Application.Services;

namespace FAFS;

/* Inherit your application services from this class.
 */
public abstract class FAFSAppService : ApplicationService
{
    protected FAFSAppService()
    {
        LocalizationResource = typeof(FAFSResource);
    }
}
