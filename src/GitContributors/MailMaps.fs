module MailMaps

open System.IO
open System.Text.RegularExpressions
open Contributor

type MailMapForm1 = {
  ToName : string
  FromEmail : string
}
type MailMapForm1 with
  static member FromLine s : MailMapForm1 option =
    let r = Regex("^(?<ToName>.*)\s+<(?<ToEmail>.*)>\s*$")
    let m = r.Match s
    if m.Success then
      {
         ToName = m.Groups.["ToName"].Value
         FromEmail = m.Groups.["ToEmail"].Value
      }
      |> Some
    else None
 
type MailMapForm2 = {
  ToEmail : string
  FromEmail : string
}
type MailMapForm2 with
  static member FromLine s : MailMapForm2 option =
    let r = Regex("^<(?<ToEmail>.*)>\s+<(?<FromEmail>.*)>\s*$")
    let m = r.Match s
    if m.Success then
      {
        ToEmail = m.Groups.["ToEmail"].Value
        FromEmail = m.Groups.["FromEmail"].Value
      }
      |> Some
    else None

type MailMapForm3 = {
  ToName : string
  ToEmail : string
  FromEmail : string
}
type MailMapForm3 with
  static member FromLine s : MailMapForm3 option =
    let r = Regex("^(?<ToName>.*)\s+<(?<ToEmail>.*)>\s+<(?<FromEmail>.*)>\s*$")
    let m = r.Match s
    if m.Success then
      {
        ToName = m.Groups.["ToName"].Value
        ToEmail = m.Groups.["ToEmail"].Value
        FromEmail = m.Groups.["FromEmail"].Value
      }
      |> Some
    else None

type MailMapForm4 = {
  ToName : string
  ToEmail : string
  FromName: string
  FromEmail : string
}

type MailMapForm4 with
  static member FromLine s =
    let r = Regex("^(?<ToName>.*)\s+<(?<ToEmail>.*)>\s+(?<FromName>.*)\s+<(?<FromEmail>.*)>\s*$")
    let m = r.Match s
    if m.Success then
      {
        ToName = m.Groups.["ToName"].Value
        ToEmail = m.Groups.["ToEmail"].Value
        FromName = m.Groups.["FromName"].Value
        FromEmail = m.Groups.["FromEmail"].Value
      }
      |> Some
    else None

type MailMap =
  | MailMapForm1 of MailMapForm1
  | MailMapForm2 of MailMapForm2
  | MailMapForm3 of MailMapForm3
  | MailMapForm4 of MailMapForm4

type MailMap with
  static member FromLine s : MailMap option =
    [
      MailMapForm4.FromLine s |> Option.map MailMap.MailMapForm4
      MailMapForm3.FromLine s |> Option.map MailMap.MailMapForm3
      MailMapForm2.FromLine s |> Option.map MailMap.MailMapForm2
      MailMapForm1.FromLine s |> Option.map MailMap.MailMapForm1
    ]
    |> Seq.choose id
    |> Seq.tryHead

  static member FromFile path =
    File.ReadLines(path)
    |> Seq.map (fun l -> if l.StartsWith("#") then None else l |> Some)
    |> Seq.map (Option.bind MailMap.FromLine)

  static member FromContributors (target : Contributor.Contributor) (source : Contributor.Contributor) =
    MailMap.MailMapForm4 {
      FromName = source.Name
      FromEmail = source.Email
      ToName = target.Name
      ToEmail = target.Email
    }
    
  static member FromContributorsArray (target : Contributor.Contributor) (sources : Contributor.Contributor array) =
    let mailMapFrom = MailMap.FromContributors target
    sources |> Array.map mailMapFrom

  static member Format(m : MailMap) =
    match m with
    | MailMapForm1 m -> $"{m.ToName} {m.FromEmail}"
    | MailMapForm2 m -> $"<{m.ToEmail}> <{m.FromEmail}>"
    | MailMapForm3 m -> $"{m.ToName} <{m.ToEmail}> <{m.FromEmail}>"
    | MailMapForm4 m -> $"{m.ToName} <{m.ToEmail}> {m.FromName} <{m.FromEmail}>"

let mergeMailMaps contributors mailMaps =

  // create updated contributors according to the mailmaps
  let mapContributor mailMap (ci : ContributorInfo) =
    let eq s1 s2 = System.String.Compare(s1, s2, true) = 0
    match mailMap with
    | MailMapForm1 m -> if eq m.FromEmail ci.Contributor.Email then { ci with Contributor = { ci.Contributor with Name = m.ToName } } else ci
    | MailMapForm2 m -> if eq m.FromEmail ci.Contributor.Email then { ci with Contributor = { ci.Contributor with Email = m.ToEmail } } else ci
    | MailMapForm3 m -> if eq m.FromEmail ci.Contributor.Email then { ci with Contributor = { Name = m.ToName; Email = m.ToEmail } } else ci
    | MailMapForm4 m -> if eq m.FromEmail ci.Contributor.Email && eq m.FromName ci.Contributor.Name then { ci with Contributor = { Name = m.ToName; Email = m.ToEmail } } else ci

  let mapContributor (mailMaps : MailMap array) (ci : ContributorInfo) =
     mailMaps
     |> Array.fold (fun ci m -> mapContributor m ci) ci
  
  let updatedContributors =
    contributors
    |> Array.map (mapContributor mailMaps)
  
  // merge all contributors that are now the same
  updatedContributors
  |> Array.groupBy (fun ci -> ci.Contributor)
  |> Array.map (fun (c,ci) -> { ci.[0] with Stats = { Authored = ci |> Array.sumBy (fun ci -> ci.Stats.Authored); Committed = ci |> Array.sumBy (fun ci -> ci.Stats.Committed) } } )

