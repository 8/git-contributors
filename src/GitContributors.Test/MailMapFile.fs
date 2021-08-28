module Test.MailMapFile

open NUnit.Framework
open FsUnit
open MailMapFile

let repoPath = ""

[<Test>]
let ``MailMapFile.fromPath()`` () =

  let mailMapFile = MailMapFile.FromPath repoPath
 
  mailMapFile |> Option.iter (fun s -> printfn $"MailMapFile: {s}")



