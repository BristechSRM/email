module CommsClient
open System
open System.Net
open System.Net.Http
open System.Configuration
open Newtonsoft.Json

let url = 
    let configUrl = ConfigurationManager.AppSettings.Item("CommsUrl")
    if String.IsNullOrWhiteSpace configUrl then
        failwith "Url for comms service is missing from configuration. Add url to procede."
    else 
        configUrl

let getKnownExternalIds email = 
    use client = new HttpClient()
    let idsUrl = sprintf "%s/ExternalIds/?handle=%s" url email
    let result = client.GetAsync(idsUrl).Result
    let json = result.Content.ReadAsStringAsync().Result
    let ids = JsonConvert.DeserializeObject<String []>(json)
    ids 
