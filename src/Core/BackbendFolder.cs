using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core;

public class BackbendFolder {
    [XmlAttribute("name")]
    public string Name { get; set; }

    private IFolder _Folder;
    public void SetFolder(IFolder folder) { _Folder = folder; }
    public IFolder GetFolder() { return _Folder; }
}