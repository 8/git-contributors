module Actions.Render

open Contributor
open Parameters
open GitContributors

module Stdio =
  let formatContributor separator c =
    let fields = [
      c.Contributor.Name
      $"{c.Contributor.Email}"
      $"{c.Stats.Authored}"
      $"{c.Stats.Committed}"
    ]
    System.String.Join(separator, fields)

  let formatHeader (s : string) (fields : string array) =
    System.String.Join(s, fields)

  let printContributors io p contributors =
    let separator = p.Separator |> Option.defaultValue "\t"
    let printfn (s:string) = io.Out.WriteLine(s)
    printfn (formatHeader separator [| "Name"; "Email"; "Committed"; "Authored"; |] )
    contributors
    |> Seq.map (formatContributor separator)
    |> Seq.iter printfn

module Console =

  open Spectre.Console

  module Table =

    let tableFrom contributors =
      let t = Table().AddColumns([|"Name"; "Email"; "Committed"; "Authored"|])
      
      let row c =
        [| c.Contributor.Name; c.Contributor.Email; $"{c.Stats.Committed}"; $"{c.Stats.Authored}"|]
        |> Array.map Markup.Escape

      contributors
      |> Array.map row
      |> Array.iter (fun r -> t.AddRow(r) |> ignore)
      t

    let printContributors io p contributors =
      contributors |> tableFrom |> AnsiConsole.Render
      ()

  module Barchart =

    type StatType = Committed | Authored

    let barchartFrom p contributors =

      let statType =
        match p.SortCriteria with 
        | Some SortCriteria.Committed -> StatType.Committed
        | _ -> StatType.Authored
      
      let b = new BarChart()
      b.Label <- statType.ToString()
      contributors
      |> Array.map (fun c ->
        let name  = c.Contributor.Name  |> Markup.Escape
        let email = c.Contributor.Email |> Markup.Escape
        $"{name} <{email}>",
        match statType with 
        | StatType.Committed -> c.Stats.Committed 
        | _ -> c.Stats.Authored
        )
      |> Array.map (fun (l,v) -> new BarChartItem(l, float v) :> IBarChartItem)
      |> fun items -> b.Data.AddRange(items)
      b
    
    let printContributors io p contributors =
      barchartFrom p contributors
      |> AnsiConsole.Render

//module Csv =
//  let printContributors io p contributors =
//    io.Error.WriteLine ("OutputFormat.Csv is not implemented!")
//    ()

let printContributors io p contributors =
  
  let print =
    match p.OutputFormat with
    | Some OutputFormat.ConsoleTable -> Console.Table.printContributors
    | Some OutputFormat.ConsoleBarchart -> Console.Barchart.printContributors
    //| Some OutputFormat.Csv -> Csv.printContributors
    | None | Some OutputFormat.Stdio -> Stdio.printContributors

  print io p contributors