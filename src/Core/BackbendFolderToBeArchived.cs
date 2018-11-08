using System;
using System.ComponentModel.DataAnnotations;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class BackbendFolderToBeArchived {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public BackbendFolder Folder { get; set; }
        public string Reason { get; set; }
    }
}
