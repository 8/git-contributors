module Contributor.Test

open System.Diagnostics
open NUnit.Framework
open FsUnit

let measure f =
  let sw = Stopwatch.StartNew ()
  f (), sw.Elapsed

let printfn (s : string) = TestContext.WriteLine s
let printf (s : string) = TestContext.Write s

[<Test>]
let ``Load Contributors from Repo`` () =
  
  // arrange
  let repoPath = """D:\downloads\repositories\haf\http.fs"""
  
  // act
  use repo = new LibGit2Sharp.Repository(repoPath)
  let result, elapsed = measure (fun () -> repo.Commits |> ContributorInfo.FromCommits)
  
  // assert
  result.Length |> should equal 38
  printfn $"Calculating Contributors took {elapsed.TotalMilliseconds}ms"
  
  ()


