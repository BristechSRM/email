module CommsMapper

open System
open Entities
open MimeKit

let firstAddress (addresses : InternetAddressList) = (addresses |> Seq.head) :?> MailboxAddress  
let firstEmail (addresses : InternetAddressList) = (firstAddress addresses).Address

let convertToISO8601 (datetime : DateTimeOffset) =
    datetime.ToString("yyyy-MM-ddTHH\:mm\:ss\Z")

let getProfileId (handles : HandleEntity []) (email : string) =
    handles 
    |> Seq.choose(fun x -> if x.Identifier.ToUpperInvariant() = email.ToUpperInvariant() then Some x.ProfileId else None)
    |> Seq.tryHead

//TODO for inbox we know the receiver all the time, for Sent items, we know sender all the time. Here we check the full handles every mapping. Update so that extra work is removed. 

let chooseCorrespondenceItem (handles : HandleEntity []) (message : MimeMessage) =  
    let senderHandle = message.From |> firstEmail
    let receiverHandle = message.To |> firstEmail

    match getProfileId handles senderHandle with
    | None -> None
    | Some senderId -> 
        match getProfileId handles receiverHandle with 
        | None -> None
        | Some receiverId ->      
            Some { Id = Guid.Empty.ToString()
                   ExternalId = message.MessageId
                   SenderId = senderId
                   ReceiverId = receiverId
                   Date = convertToISO8601 message.Date
                   Message = sprintf "Subject: %s \n Message: \n %s" message.Subject message.TextBody
                   Type = "Email"
                   SenderHandle = senderHandle
                   ReceiverHandle = receiverHandle }