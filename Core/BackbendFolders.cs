using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    [XmlRoot("BackbendFolders", Namespace = "http://www.aspenlaub.net")]
    public class BackbendFolders : List<BackbendFolder>, ISecretResult<BackbendFolders> {
        public BackbendFolders Clone() {
            var clone = new BackbendFolders();
            clone.AddRange(this);
            return clone;
        }

        public IEnumerable<BackbendFolder> FoldersOnThisMachine() {
            var machine = System.Environment.MachineName.ToLower();
            return this.Where(f => f.Machine.ToLower() == machine);
        }
    }
}
