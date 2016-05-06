module Entities

[<CLIMutable>]
type Message = 
    { Id : string
      Subject : string
      TextBody : string
      FromEmail : string
      ToEmail : string }

