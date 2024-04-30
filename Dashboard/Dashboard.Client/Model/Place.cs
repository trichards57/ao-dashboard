namespace Dashboard.Client.Model;

public class Place
{
    public Region Region { get; set; }
    public string District { get; set; } = "";
    public string Hub { get; set; } = "";

    public string CreateQuery()
    {
        if (Region == Region.All)
        {
            return string.Empty;
        }

        if (District.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return $"?region={Region}";
        }

        if (Hub.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return $"?region={Region}&district={District}";
        }

        return $"?region={Region}&district={District}&hub={Hub}";
    }
}
