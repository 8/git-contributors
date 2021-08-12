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
  let repo = """D:\downloads\repositories\haf\http.fs"""
  
  // act
  let result, elapsed = measure (fun () -> ContributorInfo.FromRepo repo)
  
  // assert
  result.Length |> should equal 38
  printfn $"Calculating Contributors took {elapsed.TotalMilliseconds}ms"
  
  ()


