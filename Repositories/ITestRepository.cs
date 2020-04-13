using Barkodai.Models;
using System.Collections.Generic;

namespace Barkodai.Repositories
{
    public interface ITestRepository
    {
        List<MyTest> GetTests();
    }
}
