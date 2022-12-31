namespace MyApp

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/charts">] Charts
    | [<EndPoint "/forms">] Forms

module Templating =
    let MenuBar (ctx: Context<EndPoint>) endpoint isMainMenuBar =
        let ( => ) (txt: string, icon: string) act =
            Templates.MainTemplate.MenuItem()
                .Title(txt)
                .ExtraCSSClasses(if endpoint = act then "active-nav-link" else "opacity-75 hover:opacity-100")
                .IconFaClass(icon)
                .TargetUrl(ctx.Link act)
                .PY(string (if isMainMenuBar then 4 else 2))
                .PL(string (if isMainMenuBar then 6 else 4))
                .Doc()
        [
            ("Home", "calendar") => EndPoint.Home
            ("Charts", "tachometer-alt") => EndPoint.Charts
            ("Forms", "align-left") => EndPoint.Forms
        ]

    /// Returns an HTML page based on the master template, given the endpoint to it, its title and body content.
    let Main ctx action (title: string) (body: Doc list) =
        Content.Page(
            Templates.MainTemplate()
                .NewPage(fun e -> JavaScript.JS.Alert "Add a nice popup here...")
                .Title(title)
                .MenuBar(MenuBar ctx action true)
                .MenuBarHamburger(MenuBar ctx action false)
                .Body(body)
                .Doc()
        )

module Site =
    open type WebSharper.UI.ClientServer

    let pageTitle (title: string) =
        Templates.MainTemplate.BodyHeader()
            .Title(title)
            .Doc()

    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            pageTitle "Home"
            Templates.MainTemplate.Grid()
                .Columns(string 1)
                .GridCards([
                    Templates.MainTemplate.GridCard()
                        .Title("Registered users")
                        .Content(hydrate (Home.Client.UserTable()))
                        .Doc()
                ])
                .Doc()
        ]

    let ChartsPage ctx =
        Templating.Main ctx EndPoint.Charts "Charts" [
            pageTitle "Charts"
            Templates.MainTemplate.Grid()
                .Columns(string 2)
                .GridCards([
                    Templates.MainTemplate.GridCard()
                        .Title("Activities")
                        .Content(client (Charts.Chart01()))
                        .Doc()
                    Templates.MainTemplate.GridCard()
                        .Title("Heartbeat")
                        .Content(client (Charts.Chart02()))
                        .Doc()
                ])
                .Doc()
        ]

    let FormsPage ctx =
        Templating.Main ctx EndPoint.Forms "Forms" [
            pageTitle "Forms"
            Templates.MainTemplate.Grid()
                .Columns(string 2)
                .GridCards([
                    Templates.MainTemplate.GridCard()
                        .Content(hydrate (Forms.CustomerInformation()))
                        .Doc()
                ])
                .Doc()
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.Charts -> ChartsPage ctx
            | EndPoint.Forms -> FormsPage ctx
        )
