open System
open System.Threading

[<EntryPoint>]
let main _ = 

    let mutable polling = true
    let intervalSeconds = 60
    let interval = intervalSeconds * 1000
    let client = ZohoClient.getConnectedClient()
    let inbox = ZohoClient.openInbox client

    while polling do 
        printfn "Starting Polling at %A" DateTime.UtcNow
        ZohoClient.getAllMessages inbox
        |> Seq.map Mapper.mimeMessageToEntity
        |> Seq.iter (InMemoryRepository.addIfNew >> ignore)
        printfn "Polling completed. Pausing at %A. \n Polling will start again in %i seconds" DateTime.UtcNow intervalSeconds
        Thread.Sleep(interval)

    client.Disconnect(true)
    let waitIndefinitelyWithToken = 
        let cancelSource = new CancellationTokenSource()
        cancelSource.Token.WaitHandle.WaitOne() |> ignore
    0
