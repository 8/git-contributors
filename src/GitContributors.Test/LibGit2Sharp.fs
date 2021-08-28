module Test.LibGit2Sharp

open FsUnit
open NUnit.Framework
open LibGit2Sharp

let printCommit (c:Commit) =
  printfn $"{c.Sha}\n{c.Author.Name} <{c.Author.Email}>\n{c.Message}\n"
  
[<Test>]
let ``get commits`` () =
  use repo = new Repository(repoPath)
  let commits = repo.Commits |> Seq.toArray
  printfn $"Commits: {commits.Length}\n"
  commits |> Seq.iter printCommit

[<Test; IgnoreInCi>]
let ``filter commits from branch`` () =
  use repo = new Repository(repoPath)
  let branch = repo.Branches.["master"]
  let filter = CommitFilter(IncludeReachableFrom = branch)
  let commits = repo.Commits.QueryBy(filter) |> Seq.toArray

  printfn $"Commits: {commits.Length}\n"
  commits |> Seq.iter printCommit

[<Test>]
let ``lookup`` () =
  use repo = new Repository(repoPath)

  let lookup (s:string) = repo.Lookup(s)
  let printObj obj = printfn $"{obj}"

  let show = lookup >> printObj

  show "master"
  show "refs/heads/master"
  show "aa46"
  show "1f7dda"
  
[<Test>]
let ``lookup non-existing object returns null`` () =
  use repo = new Repository(repoPath)

  let obj = repo.Lookup("blablabla")

  obj |> should equal null
  