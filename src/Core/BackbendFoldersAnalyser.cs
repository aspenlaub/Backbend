using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core {
    public class BackbendFoldersAnalyser {
        private const int ArchiveWithinHowManyDays = 28;
        protected IComponentProvider ComponentProvider;
        protected ISecretRepository SecretRepository;

        public BackbendFoldersAnalyser(IComponentProvider componentProvider) {
            ComponentProvider = componentProvider;
            SecretRepository = ComponentProvider.SecretRepository;
        }

        public async Task<IEnumerable<BackbendFolderToBeArchived>> AnalyseAsync(IErrorsAndInfos errorsAndInfos) {
            var result = new List<BackbendFolderToBeArchived>();
            var backbendFolders = await SecretRepository.GetAsync(new BackbendFoldersSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return result; }
            var archiveFolderFinder = await SecretRepository.GetAsync(new ArchiveFolderFinderSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return result; }

            foreach (var backbendFolder in backbendFolders.FoldersOnThisMachine()) {
                await AnalyseFolderAsync(backbendFolder, archiveFolderFinder, result);
                foreach (var subFolder in Directory.GetDirectories(backbendFolder.Name).Select(f => new BackbendFolder { Machine = backbendFolder.Machine, Name = f })) {
                    await AnalyseFolderAsync(subFolder, archiveFolderFinder, result);
                }
            }

            return result;
        }

        private async Task AnalyseFolderAsync(BackbendFolder folder, ICsScript archiveFolderFinder, ICollection<BackbendFolderToBeArchived> result) {
            if (!Directory.Exists(folder.Name)) { return; }

            var archiveFolder = await SecretRepository.ExecuteCsScriptAsync(archiveFolderFinder, new List<ICsScriptArgument> { new CsScriptArgument { Name = "folder", Value = folder.Name } });
            if (archiveFolder == "" || archiveFolder == folder.Name) { return; }

            if (archiveFolder == null) {
                throw new Exception("Error in archive folder finder");
            }

            if (!Directory.Exists(archiveFolder)) {
                result.Add(new BackbendFolderToBeArchived { Folder = folder, Reason = Properties.Resources.FoldersInNeedOfArchivingNoArchiveFolder });
                return;
            }

            var latestModificationTime = LatestModificationTime(folder.Name, "*.*", SearchOption.AllDirectories);
            if (latestModificationTime < new DateTime(2000, 1, 1)) {
                result.Add(new BackbendFolderToBeArchived { Folder = folder, Reason = Properties.Resources.FoldersInNeedOfArchivingNoFiles });
                return;
            }

            var minimumArchivingTime = latestModificationTime.AddDays(-ArchiveWithinHowManyDays);
            var newestArchivingTime = LatestModificationTime(archiveFolder, "*.*zip", SearchOption.TopDirectoryOnly);
            if (newestArchivingTime > minimumArchivingTime) { return; }

            result.Add(new BackbendFolderToBeArchived { Folder = folder, Reason = Properties.Resources.FoldersInNeedOfArchiving });
        }

        private static DateTime LatestModificationTime(string folder, string wildcard, SearchOption searchOption) {
            var files = Directory.GetFiles(folder, wildcard, searchOption).Select(f => File.GetLastWriteTime(f)).ToList();
            return files.Any() ? files.Max() : DateTime.MinValue;
        }
    }
}
