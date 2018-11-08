using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    [TestClass]
    public class BackbendFoldersToBeArchivedControllerTest {
        private const string BaseUrl = "http://localhost:65169/odata/BackbendFoldersToBeArchived";

        [TestMethod]
        public async Task CanGetBackbendFoldersToBeArchived() {
            using (var client = ControllerTestHelpers.CreateHttpClient()) {
                client.Timeout = TimeSpan.FromMinutes(5);
                var response = await client.GetAsync(BaseUrl);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                var responseString = await response.Content.ReadAsStringAsync();
                var backbendFoldersToBeArhived = JsonConvert.DeserializeObject<ODataResponse<BackbendFolderToBeArchived>>(responseString).Value.ToList();
                Assert.IsNotNull(backbendFoldersToBeArhived);
            }
        }
    }
}
