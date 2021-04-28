using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    [TestClass]
    public class HomeControllerTest {
        private const string BaseUrl = "http://localhost:65169/";

        [TestMethod]
        public async Task CanGetIndex() {
            using var client = ControllerTestHelpers.CreateHttpClient();
            var response = await client.GetAsync(BaseUrl);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseString.Contains("jumbotron"));
        }
    }
}
