using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Core;

[TestClass]
public class BackbendFoldersTest {
    private readonly IContainer _Container;

    public BackbendFoldersTest() {
        ContainerBuilder builder = new ContainerBuilder().UsePegh("Backbend");
        _Container = builder.Build();
    }

    [TestInitialize]
    public void Initialize() {
        string folder = BackbendFoldersSecret.DefaultFolder;
        if (Directory.Exists(folder)) { return; }

        Directory.CreateDirectory(folder);
    }

    [TestMethod]
    public async Task CanGetBackbendFoldersDefault() {
        var errorsAndInfos = new ErrorsAndInfos();
        var backbendFoldersSecret = new BackbendFoldersSecret();
        BackbendFolders backbendFolders = backbendFoldersSecret.DefaultValue;
        await backbendFolders.ResolveAsync(_Container.Resolve<IFolderResolver>(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        Assert.AreEqual(1, backbendFolders.Count);
    }

    [TestMethod]
    public async Task CanGetBackbendFolders() {
        ISecretRepository secretRepository = _Container.Resolve<ISecretRepository>();
        var backbendFoldersSecret = new BackbendFoldersSecret();
        var errorsAndInfos = new ErrorsAndInfos();
        BackbendFolders backbendFolders = await secretRepository.GetAsync(backbendFoldersSecret, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        Assert.IsNotNull(backbendFolders);
        Assert.IsTrue(backbendFolders.Count >= 2);
        BackbendFolders clone = backbendFolders.Clone();
        Assert.AreEqual(backbendFolders.Count, clone.Count);
        for(int i = 0; i < backbendFolders.Count; i ++) {
            Assert.AreEqual(backbendFolders[i].Name, clone[i].Name);
        }
        await backbendFolders.ResolveAsync(_Container.Resolve<IFolderResolver>(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
    }
}