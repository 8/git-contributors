module Parameters
open System

type Action =
  | Help
  | List

type SortDirection =
  | Ascending
  | Descending

type SortCriteria =
  | Name
  | Email
  | Authored
  | Committed

type OutputFormat =
  | Stdio
  | ConsoleTable
  | ConsoleBarchart
  | Csv

type Parameters = {
  Action : Action
  Directory : String option
  HelpDescription : String
  MailMapFile : String option
  SortDirection : SortDirection option
  SortCriteria : SortCriteria option
  OutputFormat : OutputFormat option
  OutputFile : string option
  Separator : string option
  Revision : string option
}

let fromArgs (args: string array) : Parameters =
  let mutable directory : string option = None
  let mutable action = Action.List
  let mutable mailmapFile : string option = None
  let mutable sortDirection : SortDirection option = None
  let mutable sortCriteria : SortCriteria option = None
  let mutable outputFormat : OutputFormat option = None
  let mutable outputFile : string option = None
  let mutable separator : string option = None
  let mutable revision : string option = None

  let parseSortDirection (s:string) =
    match s.ToLowerInvariant() with 
    | "d" | "desc" | "descending" -> Some SortDirection.Descending
    | "a" | "asc"  | "ascending" -> Some SortDirection.Ascending
    | _ -> None

  let parseOutputFormat (s:string) =
    match s.ToLowerInvariant() with
    | "t" | "table" -> Some OutputFormat.ConsoleTable
    | "b" | "bar" | "barchart" -> Some OutputFormat.ConsoleBarchart
    | "csv" -> Some OutputFormat.Csv
    | "stdio" -> Some OutputFormat.Stdio
    | _ -> None

  let options =
    Mono.Options.OptionSet()
      .Add("directory|d=", "Sets the path to the git directory, (default: '')", (fun s -> directory <- Some s))
      .Add("help|h|?", "Shows this help description.", fun s -> action <- Action.Help)
      .Add("mailmap-file|f=", "Sets the mailmap file, default: .mailmap", fun s -> mailmapFile <- Some s)
      .Add("sort-direction|sort-dir=", "Specify the SortDirection, possible values are 'asc' or 'desc'", fun s -> sortDirection <- parseSortDirection s)
      .Add("sort-desc|desc", "Sorts contributors in descending order", fun s -> sortDirection <- Some SortDirection.Descending)
      .Add("sort-asc|asc", "Sorts contributors in ascending order", fun s -> sortDirection <- Some SortDirection.Ascending)
      .Add("sort-name|sn", "Sorts contributors by Name", fun s -> sortCriteria <- Some SortCriteria.Name)
      .Add("sort-email|se", "Sorts contributors by Email", fun s -> sortCriteria  <- Some SortCriteria.Email)
      .Add("sort-authored|sa", "Sorts contributors by the number of authored commits", fun s -> sortCriteria <- Some SortCriteria.Authored)
      .Add("sort-committed|sc", "Sorts contributors by the number of committed commits", fun s -> sortCriteria <- Some SortCriteria.Committed)
      .Add("format=", "Selects the output format. Options: 'stdio (default), table, barchart or csv'", fun s -> outputFormat <- parseOutputFormat s)
      .Add("table", "Selects output format 'table'", fun s -> outputFormat <- Some OutputFormat.ConsoleTable)
      .Add("barchar|bar", "Selects output format 'barchart'", fun s -> outputFormat <- Some OutputFormat.ConsoleBarchart)
      .Add("csv", "Selects output format 'csv'", fun s -> outputFormat <- Some OutputFormat.Csv)
      .Add("output|o=", "Csv Output filename (default: 'out.csv'), implies output format 'csv' ", fun s -> outputFile <- Some s; outputFormat <- Some OutputFormat.Csv)
      .Add("stdio", "Selects output format 'stdio'", fun s -> outputFormat <- Some OutputFormat.Stdio)
      .Add("separator|sep=", "Sets the separator used for stdio and csv output", fun s -> separator <- Some s)
      .Add("revision|r=", "Specified the revision from which to look for commits (defaults: HEAD)", fun s -> revision <- Some s)

  let rest = options.Parse(args)
  
  let helpDescription =
    use sw = new System.IO.StringWriter()
    options.WriteOptionDescriptions sw
    sw.ToString()
  
  {
    Action = action
    Directory = directory
    HelpDescription = helpDescription
    MailMapFile = mailmapFile
    SortDirection = sortDirection
    SortCriteria = sortCriteria
    OutputFormat = outputFormat
    OutputFile = outputFile
    Separator = separator
    Revision = revision
  }

type Parameters with
  static member FromArgs = fromArgs