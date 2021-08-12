module Contributor
open LibGit2Sharp

type Contributor = {
  Name : string
  Email : string
}

type Stats = {
  Authored : int
  Committed : int
}

type ContributorInfo = {
  Contributor : Contributor
  Stats : Stats
}

type Contributor with
  
  static member FromSignature(signature : Signature) : Contributor =
    { Name = signature.Name; Email = signature.Email }
  
  static member FromCommit(commit : Commit) =
    Contributor.FromSignature(commit.Author), Contributor.FromSignature(commit.Committer)

type ContributorInfo with
  static member FromRepo(path : string) : ContributorInfo array =
    use repo = new Repository(path)
    
    let contributors = repo.Commits |> Seq.map Contributor.FromCommit |> Seq.toArray
    let authors      = contributors |> Seq.map fst |> Seq.groupBy id |> Seq.map (fun (c,g) -> c, g |> Seq.length) |> Map
    let committers   = contributors |> Seq.map snd |> Seq.groupBy id |> Seq.map (fun (c,g) -> c, g |> Seq.length) |> Map
    
    contributors
    |> Seq.collect (fun (c,a) -> [c;a])
    |> Seq.distinct
    |> Seq.map (fun c -> {
      Contributor = c
      Stats = {
        Committed = Map.tryFind c committers |> Option.defaultValue 0
        Authored = Map.tryFind c authors |> Option.defaultValue 0
      }
    })
    |> Seq.toArray
