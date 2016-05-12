module ZohoClient

open MailKit
open MailKit.Net.Imap
open Credentials

(*
    Currently polling for received messages only
    TODO Include sent messages
*)

let getConnectedClient() = 
    let creds = Credentials.credentials
    let client = new ImapClient()
    client.Connect("imappro.zoho.com", 993, true)
    client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
    client.Authenticate(creds.Email, creds.Password)
    client

let openInbox (client : ImapClient) = 
    let inbox = client.Inbox
    inbox.Open(FolderAccess.ReadOnly) |> ignore
    inbox

let getNewMessages (folder : IMailFolder) (knownIds : string [])=
    async {
        let! summaries = folder.FetchAsync(0, -1, MessageSummaryItems.Envelope ||| MessageSummaryItems.UniqueId) |> Async.AwaitTask
        return
            summaries
            |> Seq.filter(fun x -> not <| Array.contains x.Envelope.MessageId knownIds)
            |> Seq.map(fun x -> folder.GetMessage(x.UniqueId))    
    }

let disconnect (client : ImapClient) = client.Disconnect(true)
