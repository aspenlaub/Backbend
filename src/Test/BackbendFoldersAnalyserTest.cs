using System;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Backbend.Core;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Test {
    [TestClass]
    public class BackbendFoldersAnalyserTest {
        [TestMethod]
        public void CanAnalyseBackbendFolders() {
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
            secretRepositoryMock.Setup(s => s.Get(It.IsAny<ISecret<BackbendFolders>>(), It.IsAny<IErrorsAndInfos>())).Returns(backbendFolders);
            var secret = new ArchiveFolderFinderSecret();
            secretRepositoryMock.Setup(s => s.Get(It.IsAny<ISecret<PowershellFunction<string, string>>>(), It.IsAny<IErrorsAndInfos>())).Returns(secret.DefaultValue);
            secretRepositoryMock.Setup(s => s.ExecutePowershellFunction(It.IsAny<PowershellFunction<string, string>>(), It.Is<string>(f => f == folder || f == otherFolder))).Returns(archiveFolder);
            secretRepositoryMock.Setup(s => s.ExecutePowershellFunction(It.IsAny<PowershellFunction<string, string>>(), It.Is<string>(f => f != folder && f != otherFolder))).Returns("");
            componentProviderMock.Setup(c => c.SecretRepository).Returns(secretRepositoryMock.Object);
            var sut = new BackbendFoldersAnalyser(componentProviderMock.Object);
            var errorsAndInfos = new ErrorsAndInfos();
            var result = sut.Analyse(errorsAndInfos).ToList();
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(1, result.Count);

            File.WriteAllText(archiveFileName, textFileName);

            result = sut.Analyse(errorsAndInfos).ToList();
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(0, result.Count);

            var otherTextFileName = otherFolder + "Test.txt";
            File.WriteAllText(otherTextFileName, otherTextFileName);
            File.SetLastWriteTime(otherTextFileName, DateTime.Now.AddDays(29));

            result = sut.Analyse(errorsAndInfos).ToList();
            Assert.IsFalse(errorsAndInfos.Errors.Any(), string.Join("\r\n", errorsAndInfos.Errors));
            Assert.AreEqual(1, result.Count);

            File.Delete(textFileName);
            File.Delete(archiveFileName);
            File.Delete(otherTextFileName);
            Assert.AreEqual(0, Directory.GetFiles(otherFolder).Length);
            Directory.Delete(otherFolder);
        }
    }
}
