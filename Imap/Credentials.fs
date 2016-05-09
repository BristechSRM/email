module Credentials

open System
open System.Configuration

type Credentials = 
    { Email : string
      Password : string }

let credConfigTemplate = """<?xml version="1.0" encoding="utf-8" ?>
                            <appSettings>
                                <add key ="EmailAccount" value="{EmailAccount}" />
                                <add key ="Password" value="{Password}" />
                            </appSettings>"""

let missingCredentialsMessage = 
    "EmailAccount or Password is unset. Create a Creds.config file next to the App.config with the following xml (Filling in the account and password value fields): " 
    + credConfigTemplate

let getCredentials() = 
    let creds = 
        { Email = ConfigurationManager.AppSettings.Item("EmailAccount")
          Password = ConfigurationManager.AppSettings.Item("Password") }

    if String.IsNullOrWhiteSpace creds.Email  || String.IsNullOrWhiteSpace creds.Password then 
        failwith missingCredentialsMessage
    else creds
