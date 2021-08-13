module Commits
open LibGit2Sharp

let fromRepo (repo : Repository) revision =
  // todo use revision to control the returned commits
  repo.Commits
