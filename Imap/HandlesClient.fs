module HandlesClient
open System
open System.Net
open System.Net.Http
open System.Configuration
open Newtonsoft.Json
open Entities

let url = 
    let configUrl = ConfigurationManager.AppSettings.Item("SessionsUrl")
    if String.IsNullOrWhiteSpace configUrl then
        failwith "Url for comms service is missing from configuration. Add url to proceed."
    else 
        configUrl

let getAllHandles() = 
    use client = new HttpClient()
    let handlesUrl = sprintf "%s/Handles/" url
    let result = client.GetAsync(handlesUrl).Result
    let json = result.Content.ReadAsStringAsync().Result
    let handles = JsonConvert.DeserializeObject<HandleEntity []>(json)
    handles