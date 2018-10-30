using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test {
    [TestClass]
    public class BackbendFoldersAnalyserTest {
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
                new BackbendFolder { Machine = Environment.MachineName, Name = folder },
                new BackbendFolder { Machine = Environment.MachineName, Name = archiveFolder }
            };
            var componentProviderMock = new Mock<IComponentProvider>();
            var secretRepositoryMock = new Mock<ISecretRepository>();
            secretRepositoryMock.Setup(s => s.GetAsync(It.IsAny<ISecret<BackbendFolders>>(), It.IsAny<IErrorsAndInfos>())).Returns(Task.FromResult(backbendFolders));
            var secret = new ArchiveFolderFinderSecret();
            secretRepositoryMock.Setup(s => s.GetAsync(It.IsAny<ISecret<CsScript>>(), It.IsAny<IErrorsAndInfos>())).Returns(Task.FromResult(secret.DefaultValue));
            secretRepositoryMock.Setup(s => s.ExecuteCsScriptAsync(It.IsAny<CsScript>(), It.Is<IList<ICsScriptArgument>>(a => ArgumentIsOneForTheTwoFolders(a, folder, otherFolder)))).Returns(Task.FromResult(archiveFolder));
            secretRepositoryMock.Setup(s => s.ExecuteCsScriptAsync(It.IsAny<CsScript>(), It.Is<IList<ICsScriptArgument>>(a => !ArgumentIsOneForTheTwoFolders(a, folder, otherFolder)))).Returns(Task.FromResult(""));
            componentProviderMock.Setup(c => c.SecretRepository).Returns(secretRepositoryMock.Object);
            var sut = new BackbendFoldersAnalyser(componentProviderMock.Object);
            var errorsAndInfos = new ErrorsAndInfos();
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

        private static bool ArgumentIsOneForTheTwoFolders(IEnumerable<ICsScriptArgument> arguments, string folder, string otherFolder) {
            return arguments.Any(a => a.Name == "folder" && (a.Value == folder || a.Value == otherFolder));
        }
    }
}
