module Commits.Rev.Test

open NUnit.Framework
open FsUnit

open Commits

[<Test>]
let ``from "" should be None`` () =
  let result = RevisionFilter.From ""
  result |> should equal None

[<Test>]
let ``from "dev"`` () =
  let result = RevisionFilter.From "master"
  result |> should equal (RevisionFilter.Revision { Value = "master" } |> Some)

[<Test>]
let ``from "dev..master"`` () =
  let result = RevisionFilter.From "dev..master"
  result |> should equal (RevisionFilter.RevisionRange { From = { Value = "dev" }; To = { Value = "master"} } |> Some)
