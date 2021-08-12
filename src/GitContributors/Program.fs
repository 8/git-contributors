module GitContributors.Program

open Parameters

let showError io error =
  let printfn (s:string) = io.Error.WriteLine(s)
  printfn(error)
  ()

let handleResult io result =
  match result with
  | Ok _ -> 0
  | Error (error : string) -> showError io error; 1

let run io p =
  match p.Action with
  | Action.List -> Actions.Contributors.handle io p
  | Action.Help -> Actions.Help.handle io p
  |> handleResult io
