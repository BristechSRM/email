module Mapper

open Entities
open MimeKit

let firstAddress (addresses : InternetAddressList) = (addresses |> Seq.head) :?> MailboxAddress  
let firstEmail (addresses : InternetAddressList) = (firstAddress addresses).Address
    
let mimeMessageToEntity (message : MimeMessage ) =  
    { Id = message.MessageId 
      Subject = message.Subject
      TextBody =  message.TextBody 
      HtmlBody = message.HtmlBody 
      FromEmail = message.From |> firstEmail 
      ToEmail = message.To |> firstEmail }