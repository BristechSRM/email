module Entities

open Amazon.DynamoDBv2.DataModel

[<DynamoDBTable("Messages")>]
[<CLIMutable>]
type Message = 
    { Id : string
      Subject : string
      TextBody : string
      HtmlBody : string
      FromEmail : string
      ToEmail : string }

