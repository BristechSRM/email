open System
open System.Threading
open Logging
open Serilog
open System.Configuration

[<EntryPoint>]
let main _ = 
    setupLogging()
    let client = ZohoClient.getConnectedClient()
    let inbox = ZohoClient.openInbox client
    let creds = Credentials.credentials

    let interval = 
        let raw = ConfigurationManager.AppSettings.Item("PollingInterval")
        match TimeSpan.TryParseExact(raw,"c",null) with
        | true, span -> span
        | false, _ -> 
            let message = "PollingInterval config is missing or invalid. Make sure a valid TimeSpan value has been entered"
            Log.Fatal(message)
            failwith message
            
    let createCancelSource() = new CancellationTokenSource(interval)
    let mutable cancelSource = createCancelSource() 
    
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
            |> Seq.map (CommsMapper.tryMapToEntity handles)
            |> Seq.choose id 
            //TODO Currently ignoring emails which don't have a profile match. Can we do better?
        
        CommsClient.postAllNewCorrespondence prepedMessages

        Log.Information("Polling completed. Pausing at {now}. \n Polling will start again in {interval} seconds", DateTime.UtcNow, interval)
        
        client.Idle(cancelSource.Token)
        cancelSource <- createCancelSource() 
    0
