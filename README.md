# FSharp-GeoCoding-Service
Experimenting with F# and services. Used to access Google Geocoding services (geocoding and reverse geocoding). Uses .NET Remoting to allow interfacing. 

I used .NET Remoting for communicating with the service. The service uses the Google geocoding API. I used the JsonProvider F# Type Provider to parse the response from Google. 

The following snippet shows how I used it.

```

    type GoogleGeocoder = JsonProvider<"../data/google_geocode.json", 
                                         EmbeddedResource="MyLib, google_geocode.json">
```

The JsonProvider reads an example of Google’s API JSON output from a local file and creates a type provider, GoogleGeocoder, which knows how to handle JSON with that structure. 

```

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

```

The makeRequest function takes 3 parameters; request = the formatted URL string to make an HTTP request on, success = the function to call if the request is successful, and fail = the function to call if the request fails. 

```

    let reverseSuccess (results : GoogleGeocoder.Root) = 
        results.Results.[0].FormattedAddress

    let fail (results : GoogleGeocoder.Root) =
        results.Status

```

reverseSuccess is one of my “success” functions. It goes along with a reverse geocoding request to makeRequest. So, in makeRequest, if the res.Status field == “OK”, the reverseSuccess function is called. It returns the FormattedAddress from the first Results array. 

Fail is the “fail” function. I pass this in with either geocoding or reverse geocoding request. If the res.Status field is anything other than “OK” the fail function is called. It just returns the value of the Status field.
