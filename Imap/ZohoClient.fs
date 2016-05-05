module ZohoClient

open MailKit
open MailKit.Net.Imap

let client = new ImapClient()
client.Connect("imappro.zoho.com", 993, true)
client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
client.Authenticate("chris@bris.tech", "application specific password Here")

let inbox = client.Inbox
inbox.Open(FolderAccess.ReadOnly) |> ignore

printfn "Total messages: %d" inbox.Count
printfn "Recent messages: %d" inbox.Recent

let messages = List.init inbox.Count (fun i -> inbox.GetMessage(i))

let getFirstMessage() = List.head messages

let disconnect () = client.Disconnect(true)