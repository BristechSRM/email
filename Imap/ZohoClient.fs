module ZohoClient

open MailKit
open MailKit.Search
open MailKit.Net.Imap

let getConnectedClient() = 
    let client = new ImapClient()
    client.Connect("imappro.zoho.com", 993, true)
    client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
    client.Authenticate("srm_test@bris.tech", "hnnpbzh93n6m")
    client

let openInbox (client : ImapClient) = 
    let inbox = client.Inbox
    inbox.Open(FolderAccess.ReadOnly) |> ignore
    inbox

let getAllMessages (folder : IMailFolder) = 
    folder.Search(SearchQuery.All)
    |> Seq.map (fun i-> folder.GetMessage(i))
