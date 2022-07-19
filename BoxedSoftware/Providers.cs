namespace BoxedSoftware;

public record Provider(string Name, string Assembly) 
{
    public static Provider Sqlite = new (nameof(Sqlite), typeof(Sqlite.Marker).Assembly.GetName().Name!);
    public static Provider Postgres = new (nameof(Postgres), typeof(Postgres.Marker).Assembly.GetName().Name!);
}