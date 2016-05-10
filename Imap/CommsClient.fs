module CommsClient
open System
open System.Text
open System.Net
open System.Net.Http
open System.Configuration
open Newtonsoft.Json
open Entities
open Serilog

let url = 
    let configUrl = ConfigurationManager.AppSettings.Item("CommsUrl")
    if String.IsNullOrWhiteSpace configUrl then
        failwith "Url for comms service is missing from configuration. Add url to proceed."
    else 
        configUrl

let getKnownExternalIds email = 
    use client = new HttpClient()
    let idsUrl = sprintf "%s/ExternalIds?handle=%s" url email
    let result = client.GetAsync(idsUrl).Result
    let json = result.Content.ReadAsStringAsync().Result
    let ids = JsonConvert.DeserializeObject<String []>(json)
    ids 

let postStringAsync (client: HttpClient) (uri : string) data = 
    let content = new StringContent(data,Encoding.UTF8,"application/json")
    client.PostAsync(uri,content).Result

let post (client: HttpClient) (url) inputData = 
    let data = JsonConvert.SerializeObject(inputData)
    let response = postStringAsync client url data
    let result = response.Content.ReadAsStringAsync().Result
    result

let postAllNewCorrespondence (items : CorrespondenceItemEntity seq) = 
    use client = new HttpClient()
    let correspondenceUrl = sprintf "%s/Correspondence" url

    Log.Information("Posting new Corresspondence to {url}",correspondenceUrl)
    for item in items do         
        let result = post client correspondenceUrl item
        Log.Information("Posted item with ExternalId {id} with result {result}", item.ExternalId,result)
