using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task ResolveAsync(IFolderResolver folderResolver, IErrorsAndInfos errorsAndInfos) {
            foreach (var backbendFolder in this) {
                backbendFolder.SetFolder(await folderResolver.ResolveAsync(backbendFolder.Name, errorsAndInfos));
            }
        }
    }
}
