using System.Collections.Generic;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    [XmlRoot("BackbendFolders", Namespace = "http://www.aspenlaub.net")]
    public class BackbendFolders : List<BackbendFolder>, ISecretResult<BackbendFolders> {
        public BackbendFolders Clone() {
            var clone = new BackbendFolders();
            clone.AddRange(this);
            return clone;
        }

        public void Resolve(IErrorsAndInfos errorsAndInfos) {
            var componentProvider = new ComponentProvider();
            foreach (var backbendFolder in this) {
                backbendFolder.SetFolder(componentProvider.FolderResolver.Resolve(backbendFolder.Name, errorsAndInfos));
            }
        }
    }
}
