namespace Test

  [<AutoOpen>]
  module Helper =

    let repoPath =
      NCrunch.Framework.NCrunchEnvironment.GetOriginalSolutionPath()
      |> Option.ofObj
      |> Option.defaultValue System.Environment.CurrentDirectory
      |> System.IO.Path.GetDirectoryName
      |> LibGit2Sharp.Repository.Discover
