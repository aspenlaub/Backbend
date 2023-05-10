using System.IO;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Core;

[TestClass]
public class ArchiveFolderFinderSecretTest {
    private readonly IContainer _Container;

    public ArchiveFolderFinderSecretTest() {
        var builder = new ContainerBuilder().UsePegh("Backbend", new DummyCsArgumentPrompter());
        _Container = builder.Build();
    }

    [TestMethod]
    public void CanGetDefaultArchiveFolderAndSecretGuid() {
        var secret = new ArchiveFolderFinderSecret();
        Assert.IsNotNull(secret.DefaultValue);
        Assert.IsFalse(string.IsNullOrEmpty(secret.Guid));
    }

    [TestMethod]
    public async Task CanGetArchiveFolder() {
        var secretRepository = _Container.Resolve<ISecretRepository>();
        var secret = new ArchiveFolderFinderSecret();
        var folder = BackbendFoldersSecret.DefaultFolder;
        if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
        }

        var archiveFolderFinder = await secretRepository.CompileCsLambdaAsync<string, string>(secret.DefaultValue);
        var archiveFolder = archiveFolderFinder(folder);
        Assert.IsTrue(archiveFolder.Length != 0);
        Assert.IsTrue(archiveFolder != folder);
        Assert.IsTrue(Directory.Exists(archiveFolder));
        Assert.AreEqual(0, Directory.GetFiles(archiveFolder).Length);
        Directory.Delete(archiveFolder);
        var otherArchiveFolder = archiveFolderFinder(folder);
        Assert.AreEqual(archiveFolder, otherArchiveFolder);
        Assert.IsTrue(Directory.Exists(archiveFolder));
    }
}