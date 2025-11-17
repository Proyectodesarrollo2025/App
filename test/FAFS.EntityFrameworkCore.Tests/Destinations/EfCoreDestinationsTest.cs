using FAFS.Application.Tests.Destinations;
using FAFS.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FAFS.Destinations
{
    [Collection(FAFSTestConsts.CollectionDefinitionName)]
    internal class EfCoreDestinationsTest : DestinationAppService_Tests
    {
    }
}
