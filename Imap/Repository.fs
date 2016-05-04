module Repository 
open Entities
open Amazon
open Amazon.DynamoDBv2
open Amazon.DynamoDBv2.Model
open Amazon.DynamoDBv2.DataModel

let config = new AmazonDynamoDBConfig(RegionEndpoint = RegionEndpoint.EUWest1, ServiceURL = "http://localhost:7777")
let client = new AmazonDynamoDBClient(config)
let context = new DynamoDBContext(client)

let createBasicTableRequest (name : string) = 
    CreateTableRequest(name,ResizeArray<_>([KeySchemaElement("Id",KeyType.HASH)]),ResizeArray<_>([AttributeDefinition("Id",ScalarAttributeType.S)]), ProvisionedThroughput(1L,1L))
        
let setup () = 
    let response = client.ListTables()
    let initialCount = response.TableNames.Count
    if initialCount > 0 then
        printfn "Tables have been found. Not creating any new tables"
    else 
        client.CreateTable(createBasicTableRequest "Messages") |> ignore
        //TODO IF we keep this code, add proper waiting and detection code to ensure table is created or error is recived before exit. 
        //Wait is required because db is eventually consistent, table my not be ready yet if we do an immediate check. 
        System.Threading.Thread.Sleep(6000) |> ignore

let createMessage message = 
    let messageBatch = context.CreateBatchWrite<Message>()
    messageBatch.AddPutItem(message)
    messageBatch.Execute()
    let newMessage = context.Load<Message>(message.Id)
    if box newMessage |> isNull then None else Some message.Id

let addMessageIfNew message = 
    let storedMessage = context.Load<Message>(message.Id)
    if box storedMessage |> isNull then
        printfn "Storing new message with id : %s and subject : %s" message.Id message.Subject
        createMessage message
    else 
        //TODO what do we really want to do here? Depends on how we use this function. 
        printfn "Found message with id : %s. No add performed" message.Id
        Some message.Id