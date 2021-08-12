namespace GitContributors

open System
open System.IO

type IO = {
  In : TextReader
  Out : TextWriter
  Error : TextWriter
}

module Functions =
  let fromStdIo () = { In = Console.In; Error = Console.Error; Out = Console.Out }

type IO with
  static member FromStd = Functions.fromStdIo