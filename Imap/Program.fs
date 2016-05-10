open System
open System.Threading
open Logging
open Serilog

[<EntryPoint>]
let main _ = 
    setupLogging()
    let intervalSeconds = 60
    let interval = intervalSeconds * 1000
    let client = ZohoClient.getConnectedClient()
    let inbox = ZohoClient.openInbox client

    let creds = Credentials.credentials

    //TODO question when we fetch different information

    while true do 
        Log.Information("Starting Polling at {now}", DateTime.UtcNow)

        let handles = HandlesClient.getAllHandles()
        let knownExternalIds = CommsClient.getKnownExternalIds creds.Email

        let newMessages = 
            knownExternalIds
            |> ZohoClient.getNewMessages inbox 

        let prepedMessages = 
            newMessages
            |> Seq.map (Mapper.tryMapToEntity handles)
            |> Seq.choose id 
            //TODO Currently ignoring emails which don't have a profile match. Can we do better?
        
        CommsClient.postAllNewCorrespondence prepedMessages

        Log.Information("Polling completed. Pausing at {now}. \n Polling will start again in {interval} seconds", DateTime.UtcNow, intervalSeconds)
        Thread.Sleep(interval)
    0
