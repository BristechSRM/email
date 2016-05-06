open System
open System.Threading

[<EntryPoint>]
let main _ = 

    let intervalSeconds = 60
    let interval = intervalSeconds * 1000
    let client = ZohoClient.getConnectedClient()
    let inbox = ZohoClient.openInbox client

    while true do 
        printfn "Starting Polling at %A" DateTime.UtcNow
        ZohoClient.getAllMessages inbox
        |> Seq.map Mapper.mimeMessageToEntity
        |> Seq.iter (InMemoryRepository.addIfNew >> ignore)
        printfn "Polling completed. Pausing at %A. \n Polling will start again in %i seconds" DateTime.UtcNow intervalSeconds
        Thread.Sleep(interval)
    0
