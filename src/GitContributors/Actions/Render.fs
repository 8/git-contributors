module Actions.Render

open Contributor
open Parameters
open GitContributors

module Stdio =
  let formatContributor separator c =
    let fields = [
      c.Contributor.Name
      $"{c.Contributor.Email}"
      $"{c.Stats.Committed}"
      $"{c.Stats.Authored}"
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

module Csv =
  
  open CsvHelper
  open CsvHelper.Configuration

  type CsvRecord = {
    Name : string
    Email : string
    Committed : int
    Authored : int
  }

  type CsvRecord with
    static member From c = {
        Name = c.Contributor.Name;
        Email = c.Contributor.Email;
        Committed = c.Stats.Committed;
        Authored = c.Stats.Authored
    }

  let config p =
    let c = CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
    p.Separator |> Option.iter (fun s -> c.Delimiter <- s)
    c

  let textWriter p =
    p.OutputFile
    |> Option.defaultValue "out.csv"
    |> System.IO.File.CreateText

  let printContributors io p contributors =
    use writer = new CsvWriter(textWriter p, config p)
    writer.WriteHeader<CsvRecord>()
    writer.NextRecord()
    contributors
    |> Array.map CsvRecord.From
    |> writer.WriteRecords

let printContributors io p contributors =
  
  let print =
    match p.OutputFormat with
    | Some OutputFormat.ConsoleTable -> Console.Table.printContributors
    | Some OutputFormat.ConsoleBarchart -> Console.Barchart.printContributors
    | Some OutputFormat.Csv -> Csv.printContributors
    | None | Some OutputFormat.Stdio -> Stdio.printContributors

  print io p contributors