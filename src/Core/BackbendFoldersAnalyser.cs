using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Backbend.Core;

public class BackbendFoldersAnalyser(IFolderResolver folderResolver, ISecretRepository secretRepository) : IBackbendFoldersAnalyser {
    public const int ArchiveWithinHowManyDays = 72;
    protected readonly ISecretRepository SecretRepository = secretRepository;
    protected readonly IFolderResolver FolderResolver = folderResolver;

    public async Task<IEnumerable<BackbendFolderToBeArchived>> AnalyzeAsync(IErrorsAndInfos errorsAndInfos) {
        var result = new List<BackbendFolderToBeArchived>();
        BackbendFolders backbendFolders = await SecretRepository.GetAsync(new BackbendFoldersSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return result; }
        await backbendFolders.ResolveAsync(FolderResolver, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return result; }
        CsLambda archiveFolderFinderSecret = await SecretRepository.GetAsync(new ArchiveFolderFinderSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return result; }

        Func<string, string> archiveFolderFinder = await SecretRepository.CompileCsLambdaAsync<string, string>(archiveFolderFinderSecret);
        if (archiveFolderFinder == null) {
            errorsAndInfos.Errors.Add(Properties.Resources.CouldNotCompileFolderFinder);
            return result;
        }

        foreach (BackbendFolder backbendFolder in backbendFolders) {
            AnalyzeFolderAsync(backbendFolder, archiveFolderFinder, result);
            foreach (BackbendFolder subFolder in Directory.GetDirectories(backbendFolder.GetFolder().FullName).Select(f => new BackbendFolder { Name = f })) {
                subFolder.SetFolder(new Folder(subFolder.Name));
                AnalyzeFolderAsync(subFolder, archiveFolderFinder, result);
            }
        }

        return result;
    }

    private void AnalyzeFolderAsync(BackbendFolder folder, Func<string, string> archiveFolderFinder, ICollection<BackbendFolderToBeArchived> result) {
        if (!folder.GetFolder().Exists()) { return; }

        string backbendFolderFullName = folder.GetFolder().FullName;
        string archiveFolder = archiveFolderFinder(backbendFolderFullName);
        if (archiveFolder == "" || archiveFolder == backbendFolderFullName) { return; }

        if (archiveFolder == null) {
            throw new Exception("Error in archive folder finder");
        }

        if (!Directory.Exists(archiveFolder)) {
            result.Add(new BackbendFolderToBeArchived { Folder = folder, Reason = Properties.Resources.FoldersInNeedOfArchivingNoArchiveFolder });
            return;
        }

        DateTime latestModificationTime = LatestModificationTime(backbendFolderFullName, "*.*", SearchOption.AllDirectories);
        if (latestModificationTime < new DateTime(2000, 1, 1)) {
            result.Add(new BackbendFolderToBeArchived { Folder = folder, Reason = Properties.Resources.FoldersInNeedOfArchivingNoFiles });
            return;
        }

        DateTime minimumArchivingTime = latestModificationTime.AddDays(-ArchiveWithinHowManyDays);
        DateTime newestArchivingTime = LatestModificationTime(archiveFolder, "*.*zip", SearchOption.TopDirectoryOnly);
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

        string[] files = Directory.GetFiles(folder, wildcard, SearchOption.TopDirectoryOnly);
        lastWriteTimeStamps.AddRange(files.Select(File.GetLastWriteTime).ToList());
        return lastWriteTimeStamps.Any() ? lastWriteTimeStamps.Max() : DateTime.MinValue;
    }
}