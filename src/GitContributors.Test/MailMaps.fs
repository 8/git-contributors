module Test.MailMaps

open Contributor
open NUnit.Framework
open MailMaps
open FsUnit

[<Test>]
let Parse_Linux_MailMap_File () =
  
  // arrange
  let file = "linux.mailmap"
  let expectedParsed = 377 - 11 - 1 // all lines, minus comments, minus last, empty line

  // act
  let mailMaps = file |> MailMap.FromFile |> Seq.toArray 
  let parsed = mailMaps |> Seq.choose id |> Seq.length
  
  // assert
//  printfn $"mailMaps: {mailMaps.Length}"
//  printfn $"parsed: {parsed}"
  parsed |> should equal expectedParsed
  
//  mailMaps
//  |> Seq.iter (fun m ->
//    let t =
//      match m with
//      | Some (MailMapForm1(_)) -> "1"
//      | Some (MailMapForm2(_)) -> "2"
//      | Some (MailMapForm3(_)) -> "3"
//      | Some (MailMapForm4(_)) -> "4"
//      | None -> "None"
//      
//    printfn $"{t}: {m}")

  ()

[<Test>]
let ``Parse MailMap Form1`` () =
  
  // arrange
  let line = "Proper Name <commit@email.xx>"
  
  // act
  let result = MailMapForm1.FromLine line
  
  // assert
  result |> Option.isSome |> should equal true

[<Test>]
let Parse_MailMapForm2 () =
  
  // arrange
  let line = "<proper@email.xx> <commit@email.xx>"
  
  // act
  let result = MailMapForm2.FromLine line
  
  // assert
  result |> Option.isSome |> should equal true
  
[<Test>]
let Parse_MailMapForm3 () =
  
  // arrange
  let line = "Proper Name <proper@email.xx> <commit@email.xx>"
  
  // act
  let result = MailMapForm3.FromLine line
  
  // assert
  result |> Option.isSome |> should equal true

[<Test>]
let Parse_MailMapForm4 () =
  // arrange
  let line = "Proper Name <proper@email.xx> Commit Name <commit@email.xx>"
  
  // act
  let result = MailMapForm4.FromLine line
  
  // assert
  result |> Option.isSome |> should equal true
 
[<Test>]
let ``Merge Contributors`` () =
  
  // arrange
  let contributors : ContributorInfo array = [|
    { Contributor = { Name = "Max Musterman"; Email =  "max@mustermann.test" }; Stats = { Authored = 10; Committed = 5 } }
    { Contributor = { Name = "Max"; Email =  "max@mustermann.test" }; Stats = { Authored = 2; Committed = 1 } }
  |]
  
  let mailMaps : MailMap array = [|
    MailMapForm1 { ToName = "Max Musterman"; FromEmail = "max@mustermann.test" }
  |]

  // act
  let result = mergeMailMaps contributors mailMaps

  // assert
  result.Length |> should equal 1
  result
  |> Array.head
  |> should equal
    { Contributor = { Name = "Max Musterman"; Email = "max@mustermann.test" }
      Stats = { Authored = 12; Committed = 6 } }
  
  ()