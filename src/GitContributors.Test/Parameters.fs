module Test.Parameters

open NUnit.Framework
open FsUnit
open Parameters

[<Test>]
let ``parameters from "--help"`` () =
  let result = Parameters.FromArgs [|"--help"|]
  result.Action |> should equal Action.Help

[<Test>]
let ``parameters from "-f test.mailmap"`` () =
  let result = Parameters.FromArgs [| "-f"; "test.mailmap" |]
  result.MailMapFile |> should equal (Some "test.mailmap")

[<Test>]
let ``parameters from empty args should have no default file set``() =
  let result = Parameters.FromArgs [||]
  result.MailMapFile |> should equal None

[<Test>]
let ``parameters from empty args should list`` () =
  let result = Parameters.FromArgs [||]
  result.Action |> should equal Action.List

[<Test>]
let ``parameters from "-d c:\Test"`` () =
  let result = Parameters.FromArgs [| "-d"; """c:\Test"""|]
  result.Directory |> should equal (Some """c:\Test""")

[<Test>]
let ``parameters from emty args should have no default directory`` () =
  let result = Parameters.FromArgs[||]
  result.Directory |> should equal None

[<Test>]
let ``parameters from empty args should have asc sorting`` () =
  let result = Parameters.FromArgs[||]
  result.SortDirection |> should equal None

[<Test>]
let ``parameters from "--sort-desc" should have desc sorting`` () =
  let result = Parameters.FromArgs[| "--sort-desc" |]
  result.SortDirection |> should equal (Some SortDirection.Descending)

[<Test>]
let ``parameters from "--sort-asc" should have asc sorting`` () =
  let result = Parameters.FromArgs[| "--sort-asc" |]
  result.SortDirection |> should equal (Some SortDirection.Ascending)

[<Test>]
let ``parameters from "--sort-direction asc" should have asc sorting`` () =
  let result = Parameters.FromArgs[| "--sort-direction"; "asc" |]
  result.SortDirection |> should equal (Some SortDirection.Ascending)

[<Test>]
let ``parameters from "--sort-direction desc" should have desc sorting`` () =
  let result = Parameters.FromArgs[| "--sort-direction"; "desc" |]
  result.SortDirection |> should equal (Some SortDirection.Descending)

[<Test>]
let ``parameters from empty args should no SortDirection`` () =
  let result = Parameters.FromArgs[||]
  result.SortCriteria |> should equal None

[<Test>]
let ``parameters from "--sort-name" should have SortDirection Name`` () =
  let result = Parameters.FromArgs[| "--sort-name" |]
  result.SortCriteria |> should equal (Some SortCriteria.Name)

[<Test>]
let ``parameters from "--sort-email" should have SortDirection Email`` () =
  let result = Parameters.FromArgs[|"--sort-email"|]
  result.SortCriteria |> should equal (Some SortCriteria.Email)

[<Test>]
let ``parameters from "--sort-authored" should no SortDirection Authored`` () =
  let result = Parameters.FromArgs[|"--sort-authored"|]
  result.SortCriteria |> should equal (Some SortCriteria.Authored)

[<Test>]
let ``parameters from "--sort-authored" should no SortDirection Committed`` () =
  let result = Parameters.FromArgs[|"--sort-committed"|]
  result.SortCriteria |> should equal (Some SortCriteria.Committed)

[<Test>]
let ``parameters from empty args should not set OutputFormat`` () =
  let result = Parameters.FromArgs[||]
  result.OutputFormat |> should equal None

[<Test>]
let ``parameters from "--table" should set OutputFormat to Table`` () =
  let result = Parameters.FromArgs[|"-table"|]
  result.OutputFormat |> should equal (Some OutputFormat.ConsoleTable)

[<Test>]
let ``parameters from "-bar" should set OutputFormat to Barchar`` () =
  let result = Parameters.FromArgs[|"-bar"|]
  result.OutputFormat |> should equal (Some OutputFormat.ConsoleBarchart)

[<Test>]
let ``parameters from "--csv" should set OutputFormat to Csv`` () =
  let result = Parameters.FromArgs[|"-csv"|]
  result.OutputFormat |> should equal (Some OutputFormat.Csv)

[<Test>]
let ``parameters from "--stdio" should set OutputFormat to Stdio`` () =
  let result = Parameters.FromArgs[|"-stdio"|]
  result.OutputFormat |> should equal (Some OutputFormat.Stdio)

[<Test>]
let ``parameters from "--format=csv" should set OutputFormat to Csv`` () =
  let result = Parameters.FromArgs[|"--format"; "csv"|]
  result.OutputFormat |> should equal (Some OutputFormat.Csv)

[<Test>]
let ``parameters from "--format=barchart" should set OutputFormat to Barchart`` () =
  let result = Parameters.FromArgs[|"--format"; "barchart"|]
  result.OutputFormat |> should equal (Some OutputFormat.ConsoleBarchart)

[<Test>]
let ``parameters from "--format=table" should set OutputFormat to Table`` () =
  let result = Parameters.FromArgs[|"--format"; "table"|]
  result.OutputFormat |> should equal (Some OutputFormat.ConsoleTable)

[<Test>]
let ``parameters from "--format=stdio" should set OutputFormat to Stdio`` () =
  let result = Parameters.FromArgs[|"--format"; "stdio"|]
  result.OutputFormat |> should equal (Some OutputFormat.Stdio)
  
[<Test>]
let ``parameters from empty args should not set a Separator`` () =
  let result = Parameters.FromArgs[||]
  result.Separator |> should equal None

[<Test>]
let ``parameters from "--separator=," should set Separator to ,`` () =
  let result = Parameters.FromArgs[|"--separator"; ","|]
  result.Separator |> should equal (Some ",")

[<Test>]
let ``parameter from "-o=test.csv" should set OutputFormat 'Csv' and OutputFile 'test.csv'`` () =
  let result = Parameters.FromArgs[|"-o=test.csv"|]
  result.OutputFormat |> should equal (Some OutputFormat.Csv)
  result.OutputFile |> should equal (Some "test.csv")
