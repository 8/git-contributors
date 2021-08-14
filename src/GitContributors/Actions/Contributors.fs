module Actions.Contributors

open System.IO
open MailMaps
open Contributor
open Parameters

let loadContributors repoPath revision =
  use repo = new LibGit2Sharp.Repository(repoPath)
  Commits.fromRepo repo revision
  |> ContributorInfo.FromCommits
  |> Seq.toArray

let loadMailMaps file =
  let load file =
    MailMap.FromFile file
    |> Seq.choose id
    |> Seq.toArray
  match File.Exists file with
  | true -> load file
  | false -> [||]

let repoPathFromParams p =
  p.Directory
  |> Option.defaultValue "./"
  |> LibGit2Sharp.Repository.Discover
  |> Option.ofObj

let contributorsFrom repoPath revision =
  let mailMaps =
    MailMapFile.MailMapFile.FromPath repoPath
    |> Option.map loadMailMaps
    |> Option.defaultValue [||]
  let contributors = loadContributors repoPath revision
  mergeMailMaps contributors mailMaps

let mergeContributors p =
  match repoPathFromParams p with
  | None -> Error "Not a git directory"
  | Some repoPath -> contributorsFrom repoPath p.Revision |> Ok
 

let sortContributors p contributors =
  let sort criteria =
    match criteria with
    | Some SortCriteria.Name -> Array.sortBy (fun c -> c.Contributor.Name)
    | Some SortCriteria.Email -> Array.sortBy (fun c -> c.Contributor.Email)
    | Some SortCriteria.Authored -> Array.sortBy (fun c -> c.Stats.Authored)
    | Some SortCriteria.Committed -> Array.sortBy (fun c -> c.Stats.Committed)
    | _ -> id

  let direction contributors =
    match p.SortDirection with
      | Some SortDirection.Descending -> Array.rev contributors
      | _ -> contributors

  contributors
  |> sort p.SortCriteria
  |> direction

let handle io p =
  mergeContributors p
  |> Result.map (sortContributors p)
  |> Result.map (Render.printContributors io p)
