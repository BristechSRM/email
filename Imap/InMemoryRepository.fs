module InMemoryRepository
open Entities
let private store = System.Collections.Generic.Dictionary<string, CorrespondenceItemEntity>()

let addIfNew (message : CorrespondenceItemEntity) =
    if store.ContainsKey(message.Id) then
        printfn "Found message with id %s. Not adding to store" message.Id 
        message.Id
    else 
        printfn "Storing message with id %s" message.Id
        store.Add(message.Id, message)
        message.Id

let getExternalIds() = store |> Seq.map (fun x -> x.Value.ExternalId) |> Seq.toArray