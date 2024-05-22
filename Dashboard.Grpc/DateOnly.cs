namespace Dashboard.Grpc;

public partial class DateOnly
{
    public string ToString(string r) => new System.DateOnly((int)Year, (int)Month, (int)Day).ToString(r);
}
