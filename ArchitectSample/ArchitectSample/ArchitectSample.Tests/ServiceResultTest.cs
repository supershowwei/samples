using System.Reflection;
using ArchitectSample.Protocol.Model.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchitectSample.Tests
{
    [TestClass]
    public class ServiceResultTest
    {
        [TestMethod]
        public void Test_Implicit()
        {
            ServiceResult<int, int> failure = ServiceResult.Failure("failure");
            ServiceResult<int, int> abnormal = ServiceResult.Abnormal("abnormal", 999);

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void Test_Reflection()
        {
            var failureMethod = typeof(ServiceResult<int, int>).GetMethod("Failure", BindingFlags.Public | BindingFlags.Static);
            
            var objFailure = failureMethod.Invoke(null, new[] { "failure" });

            Assert.AreEqual(objFailure.GetType(), typeof(ServiceResult<int, int>));
        }
    }
}
