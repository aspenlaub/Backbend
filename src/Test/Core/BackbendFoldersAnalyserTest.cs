using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test.Core {
    [TestClass]
    public class BackbendFoldersAnalyserTest {
        private readonly IContainer vContainer;

        public BackbendFoldersAnalyserTest() {
            var builder = new ContainerBuilder().UsePegh(new DummyCsArgumentPrompter());
            vContainer = builder.Build();
        }

        [TestMethod]
        public async Task CanAnalyseBackbendFolders() {
            var folder = BackbendFoldersSecret.DefaultFolder;
            var archiveFolder = folder.Replace(@"\Test\", @"\TestArchive\");
            if (!Directory.Exists(archiveFolder)) {
                Directory.CreateDirectory(archiveFolder);
            }
            var otherFolder = folder + @"Sub\";
            if (!Directory.Exists(otherFolder)) {
                Directory.CreateDirectory(otherFolder);
            }

            var textFileName = folder + "Test.txt";
            File.WriteAllText(textFileName, textFileName);

            var archiveFileName = archiveFolder + "Test.zip";
            File.Delete(archiveFileName);

            var backbendFolders = new BackbendFolders {
                new BackbendFolder { Name = folder },
                new BackbendFolder { Name = archiveFolder }
            };

            var errorsAndInfos = new ErrorsAndInfos();
            backbendFolders.Resolve(vContainer.Resolve<IFolderResolver>(), errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());

            var secretRepositoryMock = new Mock<ISecretRepository>();
            secretRepositoryMock.Setup(s => s.GetAsync(It.IsAny<ISecret<BackbendFolders>>(), It.IsAny<IErrorsAndInfos>())).Returns(Task.FromResult(backbendFolders));
            secretRepositoryMock.Setup(s => s.CompileCsLambdaAsync<string, string>(It.IsAny<CsLambda>())).Returns(
                Task.FromResult<Func<string, string>>(
                    s => new Folder(s).FullName == new Folder(folder).FullName ? archiveFolder : ""
                )
            );

            var secret = new ArchiveFolderFinderSecret();
            secretRepositoryMock.Setup(s => s.GetAsync(It.IsAny<ArchiveFolderFinderSecret>(), It.IsAny<IErrorsAndInfos>())).Returns(Task.FromResult(secret.DefaultValue));

            var sut = new BackbendFoldersAnalyser(vContainer.Resolve<IFolderResolver>(), secretRepositoryMock.Object);
            errorsAndInfos = new ErrorsAndInfos();
            var result = await sut.AnalyseAsync(errorsAndInfos);
            var resultList = result.ToList();
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(1, resultList.Count);

            File.WriteAllText(archiveFileName, textFileName);

            result = await sut.AnalyseAsync(errorsAndInfos);
            resultList = result.ToList();
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(0, resultList.Count);

            var otherTextFileName = otherFolder + "Test.txt";
            File.WriteAllText(otherTextFileName, otherTextFileName);
            File.SetLastWriteTime(otherTextFileName, DateTime.Now.AddDays(29));

            result = await sut.AnalyseAsync(errorsAndInfos);
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
}
