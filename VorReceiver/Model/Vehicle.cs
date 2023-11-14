using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
}
