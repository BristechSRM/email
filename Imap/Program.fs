open System

open MailKit
open MailKit.Net.Imap

[<EntryPoint>]
let main _ = 
    use client = new ImapClient()
    client.Connect("imappro.zoho.com", 993, true)
    client.AuthenticationMechanisms.Remove("XOAUTH2") |> ignore
    client.Authenticate("chris@bris.tech", "application specific password HERE")

    let inbox = client.Inbox
    inbox.Open(FolderAccess.ReadOnly) |> ignore

    printfn "Total messages: %d" inbox.Count
    printfn "Recent messages: %d" inbox.Recent

    let subjects = [0 .. inbox.Count - 1] |> List.map (fun i ->  inbox.GetMessage(i).Subject)
    let print (subject: string) = Console.WriteLine(subject)

    subjects |> List.iter print

    client.Disconnect(true)

    0
