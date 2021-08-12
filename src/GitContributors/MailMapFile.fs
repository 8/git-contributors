module MailMapFile

open System.IO
open LibGit2Sharp

module Functions =
  let mailMapPath fileName repoPath = Path.Combine (repoPath, fileName)

  /// todo: look for configure mailmap.blob

  /// looks for configured mailmap.file
  let fromGitConfig path =
    use repo = new Repository (path)
    repo.Config.Get<string>("mailmap.file")
    |> Option.ofObj
    |> Option.map (fun c -> c.Value)
    |> Option.map (fun fileName -> mailMapPath fileName path)
    |> Option.filter File.Exists

  /// looks for default mailmap file
  let fromGitWorkingDir dir =
    let defaultMailMapFileName = ".mailmap"
    let defaultMailMapPath = mailMapPath defaultMailMapFileName
    dir
    |> Option.ofObj
    |> Option.map defaultMailMapPath
    |> Option.filter File.Exists

  /// 1. checks if there is a configured mailmap.file that exists,
  /// 2. looks for the default file
  let fromRepoPath repoPath =
    use repo = new Repository(repoPath)
    repo.Info.WorkingDirectory
    |> fromGitWorkingDir
    |> Option.orElse (fromGitConfig repoPath)

  let fromPath path =
    Repository.Discover path
    |> Option.ofObj
    |> Option.bind fromRepoPath

type MailMapFile() =
  static member FromPath = Functions.fromPath