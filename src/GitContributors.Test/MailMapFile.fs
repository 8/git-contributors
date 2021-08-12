module MailMapFile.Test

open NUnit.Framework
open FsUnit

let repoPath = ""

let printfn (text : string) = TestContext.WriteLine(text)

[<Test>]
let ``MailMapFile.fromPath()`` () =

  let mailMapFile = MailMapFile.FromPath repoPath
 
  mailMapFile |> Option.iter (fun s -> printfn $"MailMapFile: {s}")



