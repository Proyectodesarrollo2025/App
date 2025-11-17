using FAFS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FAFS.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FAFSController : AbpControllerBase
{
    protected FAFSController()
    {
        LocalizationResource = typeof(FAFSResource);
    }
}
