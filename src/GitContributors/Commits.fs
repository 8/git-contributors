module Commits
open LibGit2Sharp

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
let revisionFilter =
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
  static member From s = revisionFilter s


let commitFilter (repo : Repository) revisionFilter =

  let lookup (revision : Revision) = repo.Lookup(revision.Value)

  let from, to_ =
    match revisionFilter with
    | Revision rev -> rev, None
    | RevisionRange range -> range.From, Some range.To
    |> fun (from, to_) -> lookup from, to_ |> Option.map lookup

  CommitFilter(
    IncludeReachableFrom = from,
    ExcludeReachableFrom = (to_ |> Option.defaultValue null)
  )

let commits (repo : Repository) (revision : RevisionFilter option): Commit seq =
  match revision with
  | None -> upcast repo.Commits
  | Some filter -> filter |> commitFilter repo |> repo.Commits.QueryBy |> fun r -> upcast r

let fromRepo (repo : Repository) revision =

  revision
  |> Option.bind RevisionFilter.From
  |> commits repo
