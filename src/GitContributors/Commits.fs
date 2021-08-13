module Commits
open LibGit2Sharp

module Rev =
  type Revision = {
    Value : string
  }
  type Revision with
    static member From s =
      match System.String.IsNullOrWhiteSpace s with
      | true -> None
      | false -> Some { Value = s }

  type RevisionRange = {
    From: Revision
    To: Revision
  }

  type RevisionFilter =
    | Revision of Revision
    | RevisionRange of RevisionRange

  open System.Text.RegularExpressions

  /// https://mirrors.edge.kernel.org/pub/software/scm/git/docs/gitrevisions.html
  let from =
    let r = Regex("^(?<r1>.+?)(\.\.(?<r2>.+?))?$")
    fun s ->
      let m = r.Match(s)
      match m.Success with
      | false -> None
      | true ->
        let r1 = m.Groups.["r1"].Value |> Revision.From
        let r2 = m.Groups.["r2"].Value |> Revision.From
        match r1, r2 with
        | Some r1, None -> r1 |> RevisionFilter.Revision |> Some
        | Some r1, Some r2 -> { From = r1; To = r2 } |> RevisionFilter.RevisionRange |> Some
        | _ -> None

  type RevisionFilter with
    static member From s = from s

let fromRepo (repo : Repository) revision =
  // todo use revision to control the returned commits
  let revisionFilter = revision |> Option.bind Rev.RevisionFilter.From

  repo.Commits
