using FAFS.Samples;
using Xunit;

namespace FAFS.EntityFrameworkCore.Domains;

[Collection(FAFSTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FAFSEntityFrameworkCoreTestModule>
{

}
