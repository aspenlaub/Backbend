using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Core;

[TestClass]
public class BackbendFoldersAnalyserTest {
    private readonly IContainer _Container;

    public BackbendFoldersAnalyserTest() {
        ContainerBuilder builder = new ContainerBuilder().UsePegh("Backbend");
        _Container = builder.Build();
    }

    [TestMethod]
    public async Task CanAnalyzeBackbendFolders() {
        string folder = BackbendFoldersSecret.DefaultFolder;
        string archiveFolder = folder.Replace(@"\Test\", @"\TestArchive\");
        if (!Directory.Exists(archiveFolder)) {
            Directory.CreateDirectory(archiveFolder);
        }
        string otherFolder = folder + @"Sub\";
        if (!Directory.Exists(otherFolder)) {
            Directory.CreateDirectory(otherFolder);
        }

        string textFileName = folder + "Test.txt";
        await File.WriteAllTextAsync(textFileName, textFileName);

        string archiveFileName = archiveFolder + "Test.zip";
        File.Delete(archiveFileName);

        var backbendFolders = new BackbendFolders {
            new() { Name = folder },
            new() { Name = archiveFolder }
        };

        var errorsAndInfos = new ErrorsAndInfos();
        await backbendFolders.ResolveAsync(_Container.Resolve<IFolderResolver>(), errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);

        var secretRepositoryMock = new Mock<ISecretRepository>();
        secretRepositoryMock.Setup(s => s.GetAsync(It.IsAny<ISecret<BackbendFolders>>(), It.IsAny<IErrorsAndInfos>())).Returns(Task.FromResult(backbendFolders));
        secretRepositoryMock.Setup(s => s.CompileCsLambdaAsync<string, string>(It.IsAny<CsLambda>())).Returns(
            Task.FromResult<Func<string, string>>(
                s => new Folder(s).FullName == new Folder(folder).FullName ? archiveFolder : ""
            )
        );

        var secret = new ArchiveFolderFinderSecret();
        secretRepositoryMock.Setup(s => s.GetAsync(It.IsAny<ArchiveFolderFinderSecret>(), It.IsAny<IErrorsAndInfos>())).Returns(Task.FromResult(secret.DefaultValue));

        var sut = new BackbendFoldersAnalyser(_Container.Resolve<IFolderResolver>(), secretRepositoryMock.Object);
        errorsAndInfos = new ErrorsAndInfos();
        IEnumerable<BackbendFolderToBeArchived> result = await sut.AnalyzeAsync(errorsAndInfos);
        var resultList = result.ToList();
        Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        Assert.AreEqual(1, resultList.Count);

        await File.WriteAllTextAsync(archiveFileName, textFileName);

        result = await sut.AnalyzeAsync(errorsAndInfos);
        resultList = result.ToList();
        Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        Assert.AreEqual(0, resultList.Count);

        string otherTextFileName = otherFolder + "Test.txt";
        await File.WriteAllTextAsync(otherTextFileName, otherTextFileName);
        File.SetLastWriteTime(otherTextFileName, DateTime.Now.AddDays(BackbendFoldersAnalyser.ArchiveWithinHowManyDays + 1));

        result = await sut.AnalyzeAsync(errorsAndInfos);
        resultList = result.ToList();
        Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
        Assert.AreEqual(1, resultList.Count);

        File.Delete(textFileName);
        File.Delete(archiveFileName);
        File.Delete(otherTextFileName);
        Assert.AreEqual(0, Directory.GetFiles(otherFolder).Length);
        Directory.Delete(otherFolder);
    }
}