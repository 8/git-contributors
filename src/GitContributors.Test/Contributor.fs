module Test.Contributor

open System.Diagnostics
open NUnit.Framework
open FsUnit

open Contributor

let measure f =
  let sw = Stopwatch.StartNew ()
  f (), sw.Elapsed

[<Test>]
let ``Load Contributors from Repo`` () =
  
  // arrange
  
  // act
  use repo = new LibGit2Sharp.Repository(repoPath)
  let result, elapsed = measure (fun () -> repo.Commits |> ContributorInfo.FromCommits)
  
  // assert
  result.Length |> should greaterThan 0
  printfn $"Calculating Contributors took {elapsed.TotalMilliseconds}ms"
  
  ()


