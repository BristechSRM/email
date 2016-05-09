module ZohoClient

open MailKit
open MailKit.Search
open MailKit.Net.Imap
open Credentials

let getConnectedClient() = 
    let creds = Credentials.getCredentials()
    let client = new ImapClient()
    client.Connect("imappro.zoho.com", 993, true)
    client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
    client.Authenticate(creds.Email, creds.Password)
    client

let openInbox (client : ImapClient) = 
    let inbox = client.Inbox
    inbox.Open(FolderAccess.ReadOnly) |> ignore
    inbox

let getAllMessages (folder : IMailFolder) = 
    folder.Search(SearchQuery.All)
    |> Seq.map (fun i-> folder.GetMessage(i))

let disconnect (client : ImapClient) = client.Disconnect(true)
