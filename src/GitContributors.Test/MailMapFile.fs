module Test.MailMapFile

open NUnit.Framework
open FsUnit
open MailMapFile

let repoPath = ""

let printfn (text : string) = TestContext.WriteLine(text)

[<Test>]
let ``MailMapFile.fromPath()`` () =

  let mailMapFile = MailMapFile.FromPath repoPath
 
  mailMapFile |> Option.iter (fun s -> printfn $"MailMapFile: {s}")



