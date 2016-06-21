module InMemoryRepository
open Entities

let private store = System.Collections.Generic.Dictionary<string, Message>()

let addIfNew (message : Message) =
    if store.ContainsKey(message.Id) then
        printfn "Found message with id %s. Not adding to store" message.Id 
        message.Id
    else 
        printfn "Storing message with id %s" message.Id
        store.Add(message.Id, message)
        message.Id