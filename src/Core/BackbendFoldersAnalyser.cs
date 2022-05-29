using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core;

public class BackbendFoldersAnalyser : IBackbendFoldersAnalyser {
    public const int ArchiveWithinHowManyDays = 72;
    protected readonly ISecretRepository SecretRepository;
    protected readonly IFolderResolver FolderResolver;

    public BackbendFoldersAnalyser(IFolderResolver folderResolver, ISecretRepository secretRepository) {
        FolderResolver = folderResolver;
        SecretRepository = secretRepository;
    }

    public async Task<IEnumerable<BackbendFolderToBeArchived>> AnalyseAsync(IErrorsAndInfos errorsAndInfos) {
        var result = new List<BackbendFolderToBeArchived>();
        var backbendFolders = await SecretRepository.GetAsync(new BackbendFoldersSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return result; }
        await backbendFolders.ResolveAsync(FolderResolver, errorsAndInfos);
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
        var lastWriteTimeStamps = new List<DateTime>();
        if (searchOption == SearchOption.AllDirectories) {
            var folders = Directory.GetDirectories(folder)
                .Where(f => !f.EndsWith(@"\tools"))
                .Where(f => !f.EndsWith(@"\bin"))
                .Where(f => !f.EndsWith(@"\obj"))
                .Where(f => !f.EndsWith(@"\.git"))
                .Where(f => !f.EndsWith(@"\.vs"))
                .Where(f => !f.EndsWith(@"\.idea"))
                .Where(f => !f.EndsWith(@"\TestResults"))
                .ToList();
            lastWriteTimeStamps.AddRange(folders.Select(f => LatestModificationTime(f, wildcard, SearchOption.AllDirectories)));
        }

        var files = Directory.GetFiles(folder, wildcard, SearchOption.TopDirectoryOnly);
        lastWriteTimeStamps.AddRange(files.Select(f => File.GetLastWriteTime(f)).ToList());
        return lastWriteTimeStamps.Any() ? lastWriteTimeStamps.Max() : DateTime.MinValue;
    }
}