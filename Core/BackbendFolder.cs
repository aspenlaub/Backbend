using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class BackbendFolder {
        [XmlAttribute("machine")]
        public string Machine { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
