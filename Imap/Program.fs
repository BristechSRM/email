[<EntryPoint>]
let main _ = 
    //NOTE Hitting local dynamodb. Must be exposed at "http://localhost:7777"
    //Repository.setup()

    ZohoClient.getFirstMessage()
    |> Mapper.mimeMessageToEntity
    |> Repository.addMessageIfNew
    |> Option.iter (printfn "Id of first stored message: %s")

    ZohoClient.disconnect()
    0
