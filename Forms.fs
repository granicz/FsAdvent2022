namespace MyApp

open WebSharper
open WebSharper.UI
open WebSharper.UI.Templating

[<JavaScript>]
module Templates =
    type MainTemplate = Templating.Template<"Main.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>

[<JavaScript>]
module Forms =
    open WebSharper.Forms
    open WebSharper.JavaScript

    let CustomerInformation () =
        if IsClient then
            Form.Return (fun name email street city country zip cc ->
                name, email, street, city, country, zip, cc)
            <*> Form.Yield ""
            <*> Form.Yield ""
            <*> Form.Yield ""
            <*> Form.Yield ""
            <*> Form.Yield ""
            <*> Form.Yield ""
            <*> Form.Yield ""
            |> Form.WithSubmit
            |> Form.Run (fun ((name, email, street, city, country, zip, cc) as data) ->
                JS.Alert $"You submitted: {data}"
            )
            |> Form.Render (fun name email street city country zip cc submitter ->
                Templates.MainTemplate.CheckoutForm()
                    .Name(name)
                    .Email(email)
                    .Street(street)
                    .City(city)
                    .Country(country)
                    .Zip(zip)
                    .CreditCard(cc)
                    .Submit(fun e -> submitter.Trigger())
                    .Doc()
            )
        else
            Templates.MainTemplate.CheckoutForm()
                .Name("")
                .Email("")
                .Street("")
                .City("")
                .Country("")
                .Zip("")
                .CreditCard("")
                .Doc()
