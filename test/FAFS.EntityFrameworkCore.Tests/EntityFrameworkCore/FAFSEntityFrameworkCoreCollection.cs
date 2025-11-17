using Xunit;

namespace FAFS.EntityFrameworkCore;

[CollectionDefinition(FAFSTestConsts.CollectionDefinitionName)]
public class FAFSEntityFrameworkCoreCollection : ICollectionFixture<FAFSEntityFrameworkCoreFixture>
{

}
