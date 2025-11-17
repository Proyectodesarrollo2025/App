using FAFS.Samples;
using Xunit;

namespace FAFS.EntityFrameworkCore.Applications;

[Collection(FAFSTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FAFSEntityFrameworkCoreTestModule>
{

}
