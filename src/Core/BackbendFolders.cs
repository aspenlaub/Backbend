using System.Collections.Generic;
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

        public void Resolve(IFolderResolver folderResolver, IErrorsAndInfos errorsAndInfos) {
            foreach (var backbendFolder in this) {
                backbendFolder.SetFolder(folderResolver.Resolve(backbendFolder.Name, errorsAndInfos));
            }
        }
    }
}
