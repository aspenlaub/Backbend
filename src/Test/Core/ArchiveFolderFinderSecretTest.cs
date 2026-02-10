using System;
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
        ContainerBuilder builder = new ContainerBuilder().UsePegh("Backbend");
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
        ISecretRepository secretRepository = _Container.Resolve<ISecretRepository>();
        var secret = new ArchiveFolderFinderSecret();
        string folder = BackbendFoldersSecret.DefaultFolder;
        if (!Directory.Exists(folder)) {
            Directory.CreateDirectory(folder);
        }

        Func<string, string> archiveFolderFinder = await secretRepository.CompileCsLambdaAsync<string, string>(secret.DefaultValue);
        string archiveFolder = archiveFolderFinder(folder);
        Assert.IsTrue(archiveFolder.Length != 0);
        Assert.IsTrue(archiveFolder != folder);
        Assert.IsTrue(Directory.Exists(archiveFolder));
        Assert.AreEqual(0, Directory.GetFiles(archiveFolder).Length);
        Directory.Delete(archiveFolder);
        string otherArchiveFolder = archiveFolderFinder(folder);
        Assert.AreEqual(archiveFolder, otherArchiveFolder);
        Assert.IsTrue(Directory.Exists(archiveFolder));
    }
}