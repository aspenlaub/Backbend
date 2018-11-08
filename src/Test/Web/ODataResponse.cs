using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Web {
    public class ODataResponse<T> {
        public List<T> Value { get; set; }
    }
}
