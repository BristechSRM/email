open Logging
open Serilog
open SrmApiClient
open System
open System.Threading

let client = ZohoClient.getConnectedClient()
let inbox = ZohoClient.openInbox client
let creds = Credentials.credentials
let interval = Config.interval
let createCancelSource() = new CancellationTokenSource(interval)

let importNewMessages inbox (creds : Credentials.Credentials) = 
    async { 
        let! handles = Handles.getAll()
        let! knownExternalIds = ExternalIds.get creds.Email
        let! newMessages = ZohoClient.getNewMessages inbox knownExternalIds
        let prepedMessages = newMessages |> Seq.choose (CommsMapper.tryMapToEntity handles)
        //TODO Currently ignoring emails which don't have a profile match. Can we do better?
        return! Correspondence.postAll prepedMessages
    }

[<EntryPoint>]
let main _ = 
    setupLogging()
    Log.Information("Polling Interval: {interval}", interval)
    let mutable cancelSource = createCancelSource()
    while true do
        Log.Information("Starting Polling at {now}", DateTime.UtcNow)
        importNewMessages inbox creds |> Async.RunSynchronously
        Log.Information("Polling completed. Pausing at {now}. \n Polling will start again in {interval} seconds", DateTime.UtcNow, interval)
        client.Idle(cancelSource.Token)
        cancelSource <- createCancelSource()
    0
