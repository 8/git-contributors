namespace Test

  [<AutoOpen>]
  module Helper =

    let repoPath =
      NCrunch.Framework.NCrunchEnvironment.GetOriginalSolutionPath()
      |> Option.ofObj
      |> Option.defaultValue System.Environment.CurrentDirectory
      |> System.IO.Path.GetDirectoryName
      |> LibGit2Sharp.Repository.Discover

    let printfn (s:string) = NUnit.Framework.TestContext.WriteLine s 

    let printf (s : string) = NUnit.Framework.TestContext.Write s

  type IgnoreInCiAttribute() = inherit NUnit.Framework.CategoryAttribute("IgnoreInCi")