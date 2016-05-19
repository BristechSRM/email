module Entities

[<CLIMutable>]
type CorrespondenceItemEntity =
    { Id : string
      ExternalId : string
      SenderId : string
      ReceiverId : string
      Date : string
      Message : string
      Type : string
      SenderHandle : string
      ReceiverHandle : string }

[<CLIMutable>]
type HandleEntity =
    { ProfileId : string
      Type : string
      Identifier : string }
