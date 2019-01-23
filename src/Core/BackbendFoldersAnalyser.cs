﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
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
            backbendFolders.Resolve(errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return result; }
            var archiveFolderFinderSecret = await SecretRepository.GetAsync(new ArchiveFolderFinderSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return result; }

            var archiveFolderFinder = await SecretRepository.CompileCsLambdaAsync<string, string>(archiveFolderFinderSecret);
            if (archiveFolderFinder == null) {
                errorsAndInfos.Errors.Add(Properties.Resources.CouldNotCompileFolderFinder);
                return result;
            }

            foreach (var backbendFolder in backbendFolders) {
                AnalyseFolderAsync(backbendFolder, archiveFolderFinder, result);
                foreach (var subFolder in Directory.GetDirectories(backbendFolder.GetFolder().FullName).Select(f => new BackbendFolder { Name = f })) {
                    subFolder.SetFolder(new Folder(subFolder.Name));
                    AnalyseFolderAsync(subFolder, archiveFolderFinder, result);
                }
            }

            return result;
        }

        private void AnalyseFolderAsync(BackbendFolder folder, Func<string, string> archiveFolderFinder, ICollection<BackbendFolderToBeArchived> result) {
            if (!folder.GetFolder().Exists()) { return; }

            var backbendFolderFullName = folder.GetFolder().FullName;
            var archiveFolder = archiveFolderFinder(backbendFolderFullName);
            if (archiveFolder == "" || archiveFolder == backbendFolderFullName) { return; }

            if (archiveFolder == null) {
                throw new Exception("Error in archive folder finder");
            }

            if (!Directory.Exists(archiveFolder)) {
                result.Add(new BackbendFolderToBeArchived { Folder = folder, Reason = Properties.Resources.FoldersInNeedOfArchivingNoArchiveFolder });
                return;
            }

            var latestModificationTime = LatestModificationTime(backbendFolderFullName, "*.*", SearchOption.AllDirectories);
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
