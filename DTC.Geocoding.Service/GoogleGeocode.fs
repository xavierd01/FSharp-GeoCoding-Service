namespace DTC.Geocoding.Service

module GoogleGeocode =
    open FSharp.Data

    (* Google Reverse Geocoding Web Service *)
    type GoogleGeocoder = JsonProvider<"../data/google_geocode.json", 
                                         EmbeddedResource="MyLib, google_geocode.json">
    let googleGeocodeUrl =
        "https://maps.googleapis.com/maps/api/geocode/json"

    let geocodeCommand = "?address="
    let reverseGeoCommand = "?latlng="

//    type GoogleStatus =
//        | OK
//        | ZeroResults
//        | OverQueryLimit
//        | RequestDenied
//        | InvalidRequest
//        | Unknown
//        | Error

    let buildRequest baseUrl command parameters =
        List.reduce (fun acc elem -> acc + "," + elem) parameters
        |> (+) command
        |> (+) baseUrl
        //baseUrl + command + List.reduce (fun acc elem -> acc + "," + elem) parameters
    
    let geocodeRequest = buildRequest googleGeocodeUrl geocodeCommand 
    let reverseGeocodeRequest = buildRequest googleGeocodeUrl reverseGeoCommand 

    let reverseSuccess (results : GoogleGeocoder.Root) = 
        results.Results.[0].FormattedAddress

    let geocodeSuccess (results : GoogleGeocoder.Root) =
        (results.Results.[0].Geometry.Location.Lat |> string ) + ", " + (results.Results.[0].Geometry.Location.Lng |> string)

    let fail (results : GoogleGeocoder.Root) =
        results.Status

    let makeRequest request success fail =
        let data = 
            Http.RequestString
                (   request,
                    headers=["content-type", "application/json"] )
        let res = GoogleGeocoder.Parse(data)
        match res.Status with 
        | "OK" -> 
            success res
        | _ -> 
            fail res
    
    let requestReverseGeocode lat lng =
        makeRequest (reverseGeocodeRequest [lat; lng]) reverseSuccess fail

    let requestGeocode address =
        makeRequest (geocodeRequest [address]) geocodeSuccess fail
        

module GeocodeService =
    open System

    /// Specifies members that service provides
    type IGeocodeService =
        abstract Geocode : string -> string
        abstract ReverseGeocode : string -> string -> string

    /// Concrete implementation of geocode service
    type GoogleGeocodeService() =
        inherit MarshalByRefObject()
        
        interface IGeocodeService with
            member this.Geocode address = 
                GoogleGeocode.requestGeocode address
            member this.ReverseGeocode lat lng = 
                GoogleGeocode.requestReverseGeocode lat lng