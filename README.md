# Backbend

Summary: this repository contains my first attempt to use github.com for something meaningful: a little reminder application about projects that have not been archived (=zipped) for a while

## What does *Backbend* mean?

*Backbend* is just a play of words with echoes of *backup* and *backend*

## Can it be useful to you?

Yes. The folder list and the mapping function from sub folder to zip folder are [secrets](https://github.com/aspenlaub/Pegh/blob/master/README.md) which you can configure in files placed in your app data folder.

## Honorary mentions

### Class ```BackbendFolder```

Class ```BackbendFolder``` is a piece of configuration, a folder that holds projects you want to archive from time to time. The folder may contain placeholders which are resolved using Pegh's secret machine drives and logical filenames.

### Class ```BackbendFolders```

Class ```BackbendFolders``` is a list of ```BackbendFolder``` and a potential secret because it implements ```ISecretResult<BackbendFolders>``` (which means any instance is able to clone itself).

### Class ```BackbendFoldersSecret```

Class ```BackbendFoldersSecret``` implements ```ISecret<BackbendFolders>```. The secret has a GUID and a default value, which in this case is a one-element list of backbend folders.

Once the secret was accessed, you will find it stored in your *AppData\Roaming\Aspenlaub.Net\SecretRepository* folder, the secret's GUID and extension *.xml* from the file name for the secret.

### Class ```ArchiveFolderFinderSecret```

```ArchiveFolderFinderSecret``` implements ```ISecret<PowershellFunction<string, string>>```, so it is a Powershell secret, in other words: a secret algorithm.

### Class ```BackbendFoldersAnalyser```

This class does the work. It reads (or creates) your secret list of backbend folders using ```SecretRepository.Get(new BackbendFoldersSecret())``` and your secret algorithm ```SecretRepository.Get(new ArchiveFolderFinderSecret())```.

Any source code change must be archived after 28 days at the latest.
