module Test.Program

open FsUnit
open NUnit.Framework
open GitContributors

module Helper =
  let getIo () : IO =
    let stringReader = new System.IO.StringReader("");
    { 
      In = stringReader;
      Out = TestContext.Out;
      Error = TestContext.Error
    }

  let flushIo io =
    io.Out.Flush()
    io.Error.Flush()

  let withIo f =
    let io = getIo ()
    let result = f io
    flushIo io
    result

let runProgram p =
  let test io = Program.run io p
  Helper.withIo test

[<Test>]
let ``run help`` () =
  let parameters = Parameters.fromArgs([|"--help"|])
  let result = runProgram parameters
  ()

[<Test>]
let ``run with default params`` () = 
  let parameters = Parameters.fromArgs([| |])
  let result = runProgram parameters
  ()

[<Test>]
let ``run with valid git repo`` () =
  let parameters = Parameters.fromArgs([|"-d"; repoPath|])
  let result = runProgram parameters
  ()

[<Test; IgnoreInCi>]
let ``run with "-r master"`` () =
  let parameters = Parameters.fromArgs([|"-d"; repoPath; "-r"; "master"|])
  let result = runProgram parameters
  ()

[<Test; IgnoreInCi>]
let ``run with "-r master..master" `` () =
  let parameters = Parameters.fromArgs([|"-d"; repoPath; "-r"; "master..master"|])
  let result = runProgram parameters
  ()

