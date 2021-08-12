open GitContributors

[<EntryPoint>]
let main argv =
  let io = IO.FromStd()

  argv
  |> Parameters.fromArgs
  |> Program.run io
