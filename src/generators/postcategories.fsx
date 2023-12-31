#r "../_lib/Fornax.Core.dll"
#load "../globals.fsx"
#load "layout.fsx"

open Postloader
open Layout
open Html

open System.IO
open System.Diagnostics
open Globals

let generate (ctx : SiteContents) (projectRoot: string) (page: string) =

    ctx.TryGetValues<NotebookPost>() 
    |> Option.defaultValue Seq.empty
    |> List.ofSeq
    |> List.groupBy (fun post -> post.post_config.category)
    |> List.map (fun (category, posts) ->
        Path.Combine([|projectRoot; "_public"; "posts"; "categories"; $"{category}.html"|]),

        let metadata = 
            SiteMetadata.create(
                title = $"The FsLab blog - {PostCategory.toString category} posts",
                description = $"The {PostCategory.toString category} category contains {PostCategory.getDescription category}"
            )


        Layout.layout ctx metadata "Posts" [
            section [Class "hero is-small has-bg-darkmagenta"] [
                div [Class "hero-body"] [
                    div [Class "container has-text-centered"] [
                        div [Class "main-TextField"] [
                            h1 [Class "title is-capitalized is-white is-inline-block is-emphasized-aquamarine mb-4"] [!! $"{category |> PostCategory.toString} posts"]
                            h3 [Class "subtitle is-size-4 is-white mt-4"] [!! (category |> PostCategory.getDescription)]
                        ]
                    ]
                ]
            ]
            section [] [
                div [Class "container"] [
                    div [Class "timeline is-centered"] (
                        posts
                        |> List.sortByDescending (fun p -> p.post_config.date)
                        |> List.map (fun post -> 
                            div [Class"timeline-item is-darkmagenta"] [
                                div [Class "timeline-marker"] []
                                div [Class "timeline-content"] [
                                    div [Class "content"] [
                                        p [Class "heading is-size-4"] [!! $"{post.post_config.date.Year}-{post.post_config.date.Month}-{post.post_config.date.Day}"]
                                        Layout.standardPostPreview post
                                    ]
                                ]
                            ]
                        )
                    )
                ]


            ]
        ]
        |> Layout.render ctx
    )
