using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace VorReceiver.Model;

public class Incident
{
    [JsonProperty("comments")]
    public string Comments { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("endDate")]
    public DateOnly EndDate { get; set; }

    [JsonProperty("startDate")]
    public DateOnly StartDate { get; set; }

    [JsonProperty("estimatedEndDate")]
    public DateOnly? EstimatedEndDate { get; set; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum VehicleType
{
    [EnumMember(Value ="other")]
    Other = 0,
    [EnumMember(Value = "frontline")]
    FrontLineAmbulance = 1,
    [EnumMember(Value = "awd")]
    AllWheelDrive = 2,
    [EnumMember(Value = "ora")]
    OffRoadAmbulance = 3
}

[JsonConverter(typeof(StringEnumConverter))]
public enum Region
{
    [EnumMember(Value = "unknown")]
    Unknown = 0,
    [EnumMember(Value = "ne")]
    NorthEast = 1,
    [EnumMember(Value = "nw")]
    NortWest = 2,
    [EnumMember(Value = "ee")]
    EastOfEngland = 3,
    [EnumMember(Value = "wm")]
    WestMidlands = 4,
    [EnumMember(Value = "em")]
    EastMidlands = 5,
    [EnumMember(Value = "lo")]
    London = 6,
    [EnumMember(Value = "se")]
    SouthEast = 7,
    [EnumMember(Value = "sw")]
    SouthWest = 8
}

public class Vehicle
{
    [JsonProperty("callSign")]
    public string CallSign { get; set; }

    [JsonProperty("partition")]
    public string Partition => "VOR";

    [JsonProperty("incidents")]
    public List<Incident> Incidents { get; set; } = new List<Incident>();

    [JsonProperty("id")]
    public string Registration { get; set; }

    [JsonProperty("_etag")]
    public string Etag { get; set; }

    [JsonProperty("body")]
    public string BodyType { get; set; }

    [JsonProperty("make")]
    public string Make { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("isVor")]
    public bool IsVor { get; set; }

    [JsonProperty("district", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), DefaultValue("Unknown")]
    public string Distict { get; set; } = "Unknown";

    [JsonProperty("region", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), DefaultValue("Unknown")]
    public Region Region { get; set; } = Region.Unknown;

    [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate), DefaultValue(VehicleType.Other)]
    public VehicleType VehicleType { get; set; } = VehicleType.Other;
}
