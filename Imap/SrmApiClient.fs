module SrmApiClient

open System
open System.Net.Http
open Newtonsoft.Json
open Entities
open System.Text
open Serilog

let sessionsUri = Config.sessionsUri
let commsUri = Config.commsUri

let postStringAsync (client: HttpClient) (uri : Uri) data = 
    let content = new StringContent(data, Encoding.UTF8, "application/json")
    client.PostAsync(uri, content) |> Async.AwaitTask

let post (client: HttpClient) uri data = 
    async {
        let dataString = JsonConvert.SerializeObject(data)
        let! response = postStringAsync client uri dataString
        return! response.Content.ReadAsStringAsync() |> Async.AwaitTask
    }

let get (client: HttpClient) (uri : Uri) : Async<'a> =
    async {
        let! result = client.GetAsync(uri) |> Async.AwaitTask
        let! json = result.Content.ReadAsStringAsync() |> Async.AwaitTask
        return JsonConvert.DeserializeObject<'a>(json)
    }

module Handles = 
    let getAll() : Async<HandleEntity []>= 
        async {
            use client = new HttpClient()
            let handlesUri = Uri(sessionsUri,"Handles")
            return! get client handlesUri
        }

module ExternalIds = 
    let get email : Async<string []> = 
        async {
            use client = new HttpClient()
            let idsUri = Uri(commsUri, sprintf "ExternalIds?handle=%s" email)
            return! get client idsUri
        }

module Correspondence = 
    let postCorr client uri item = 
        async {
            let! result = post client uri item
            Log.Information("Posted item with ExternalId {id} with result {result}", item.ExternalId, result)
            return result
        }

    let postAll (items : CorrespondenceItemEntity seq) = 
        async {
            use client = new HttpClient()
            let correspondenceUri = Uri(commsUri, "Correspondence")
            if Seq.isEmpty items then
                Log.Information("No New Correspondence to insert")
            else 
                Log.Information("New Correspondence found. Posting to: {uri}",correspondenceUri)
                items
                |> Seq.map (postCorr client correspondenceUri)
                |> Async.Parallel
                |> Async.RunSynchronously
                |> ignore
        }