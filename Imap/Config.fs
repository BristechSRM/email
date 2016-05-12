module Config

open FSharp.Configuration
open System

//Using FSharp.Configuration type provider

type Settings = AppSettings<"App.config">

let interval : TimeSpan = Settings.PollingInterval
let sessionsUri : Uri = Settings.SessionsUrl
let commsUri : Uri = Settings.CommsUrl


