module Actions.Contributors

open System.IO
open MailMaps
open Contributor
open Parameters

let loadContributors repoPath =
  repoPath
  |> ContributorInfo.FromRepo
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

let contributorsFrom repoPath =
  let mailMaps =
    MailMapFile.MailMapFile.FromPath repoPath
    |> Option.map loadMailMaps
    |> Option.defaultValue [||]
  let contributors = loadContributors repoPath
  mergeMailMaps contributors mailMaps

let mergeContributors p =
  match repoPathFromParams p with
  | None -> Error "Not a git directory"
  | Some repoPath -> repoPath |> contributorsFrom |> Ok
 

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
