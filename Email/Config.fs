module Config

open System
open System.Configuration
open Serilog

let getConfigValue (key : string) = 
    let value = ConfigurationManager.AppSettings.Item(key)
    if String.IsNullOrWhiteSpace value then
        failwith <| sprintf "Configuration value with key %s is missing / is null  / is blank in configuration. Add key and value to proceed." key
    else 
        value

let getUriConfigValue (key : string) = 
    let value = getConfigValue key
    let errorMessage = sprintf "%s config is invalid. Make sure a valid TimeSpan value has been entered." key
    try
        Uri(value)
    with
    | :? UriFormatException as ex -> 
        let fullMessage = errorMessage + " " + ex.Message
        Log.Fatal(fullMessage)
        reraise()
    | ex -> 
        let fullMessage = errorMessage + " " + ex.Message
        Log.Fatal(fullMessage)
        reraise()    
        
let interval = 
    let key = "PollingInterval"
    let value = getConfigValue key
    match TimeSpan.TryParseExact(value,"c",null) with
    | true, span -> span
    | false, _ -> 
        let message = sprintf "%s config is invalid. Make sure a valid TimeSpan value has been entered" key
        Log.Fatal(message)
        failwith message        

let sessionsUri : Uri = getUriConfigValue "SessionsUrl"

let commsUri : Uri = getUriConfigValue "CommsUrl"


