open Logging
open Serilog
open SrmApiClient
open System
open System.Threading
open CommsMapper

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
        let items = newMessages |> Seq.choose (chooseCorrespondenceItem handles)

        return! Correspondence.postAll items
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
        (* 
            Here we must switch to the idle state because without doing so, the connection is automatically closed (probably by the Zoho server)
            The idle state keeps the connection open when we aren't actively using it. 

            Idle is a blocking method, so an asynchronous way of cancelling out of idle must be used. the cancellationToken is the easiest way to do this. 
        *)
    0
