namespace DTC.Geocoding.Service

open System;
open System.Diagnostics;
open System.Linq;
open System.ServiceProcess;
open System.Text;
open System.ServiceProcess
open System.Runtime.Remoting
open System.Runtime.Remoting.Channels

type public GeocodeWindowsService() as service =
    inherit ServiceBase()

    // TODO define your service variables
    let eventLog = new EventLog();

    // TODO initialize your service
    let initService = 
        service.ServiceName <- "DTC.GeocodeService.Prototype"

        // Define the Event Log
        let eventSource = "DTC.GeocodeService.Prototype"
        if not (EventLog.SourceExists(eventSource)) then
            EventLog.CreateEventSource(eventSource, "Application");

        eventLog.Source <- eventSource;
        eventLog.Log <- "Application";

    do
        initService

    // TODO define your service operations
    override service.OnStart(args:string[]) =
        base.OnStart(args)
        eventLog.WriteEntry("Service Started")
        let channel = new Tcp.TcpChannel(42042)
        ChannelServices.RegisterChannel(channel, false)
        RemotingConfiguration.RegisterWellKnownServiceType
            (   typeof<GeocodeService.GoogleGeocodeService>, "FsGeocodeService",
                WellKnownObjectMode.Singleton )

    override service.OnStop() =
        base.OnStop()
        eventLog.WriteEntry("Service Ended")