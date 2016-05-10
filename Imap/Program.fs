open System
open System.Threading
open Logging

[<EntryPoint>]
let main _ = 
    setupLogging()
    let intervalSeconds = 60
    let interval = intervalSeconds * 1000
    let client = ZohoClient.getConnectedClient()
    let inbox = ZohoClient.openInbox client

    let handles = HandlesClient.getAllHandles()

    while true do 
        printfn "Starting Polling at %A" DateTime.UtcNow
        let knownExternalIds = [||]
        ZohoClient.getNewMessages inbox knownExternalIds 
        |> Seq.map (Mapper.tryMapToEntity handles)
        |> Seq.choose id //TODO Currently ignoring emails which don't have a profile match. Can we do better?
        |> Seq.iter (InMemoryRepository.addIfNew >> ignore)
        printfn "Polling completed. Pausing at %A. \n Polling will start again in %i seconds" DateTime.UtcNow intervalSeconds
        Thread.Sleep(interval)
    0
