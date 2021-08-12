module Actions.Help

open Parameters
open GitContributors
open System.Reflection

let version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3)

let handle io p =
  let printfn (s: string) = io.Out.WriteLine(s)
  printfn $"git-contributor v{version} written by Martin Kramer / www.lostindetails.com / http://github.com/8/git-contributors"
  printfn $"{p.HelpDescription}"
  Ok ()
