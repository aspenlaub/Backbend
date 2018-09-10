using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public IEnumerable<string> Analyse(IErrorsAndInfos errorsAndInfos) {
            var result = new List<string>();
            var backbendFolders = SecretRepository.Get(new BackbendFoldersSecret(), errorsAndInfos);
            var archiveFolderFinder = SecretRepository.Get(new ArchiveFolderFinderSecret(), errorsAndInfos);
            foreach (var backbendFolder in backbendFolders.FoldersOnThisMachine()) {
                AnalyseFolder(backbendFolder.Name, archiveFolderFinder, result);
                foreach (var subFolder in Directory.GetDirectories(backbendFolder.Name)) {
                    AnalyseFolder(subFolder, archiveFolderFinder, result);
                }
            }

            return result;
        }

        private void AnalyseFolder(string folder, IPowershellFunction<string, string> archiveFolderFinder, ICollection<string> result) {
            if (!Directory.Exists(folder)) { return; }

            var archiveFolder = SecretRepository.ExecutePowershellFunction(archiveFolderFinder, folder);
            if (archiveFolder == "" || archiveFolder == folder) { return; }

            if (archiveFolder == null) {
                throw new Exception("Error in archive folder finder");
            }

            if (!Directory.Exists(archiveFolder)) {
                result.Add(string.Format(Properties.Resources.FolderInNeedOfArchivingNoArchiveFolder, folder));
                return;
            }

            var latestModificationTime = LatestModificationTime(folder, "*.*", SearchOption.AllDirectories);
            if (latestModificationTime < new DateTime(2000, 1, 1)) {
                result.Add(string.Format(Properties.Resources.FolderInNeedOfArchivingNoFiles, folder));
                return;
            }

            var minimumArchivingTime = latestModificationTime.AddDays(-ArchiveWithinHowManyDays);
            var newestArchivingTime = LatestModificationTime(archiveFolder, "*.*zip", SearchOption.TopDirectoryOnly);
            if (newestArchivingTime > minimumArchivingTime) { return; }

            result.Add(string.Format(Properties.Resources.FolderInNeedOfArchiving, folder));
        }

        private static DateTime LatestModificationTime(string folder, string wildcard, SearchOption searchOption) {
            var files = Directory.GetFiles(folder, wildcard, searchOption).Select(f => File.GetLastWriteTime(f)).ToList();
            return files.Any() ? files.Max() : DateTime.MinValue;
        }
    }
}
