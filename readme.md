# Multiple Database Providers with EF Core

If you need the same models and relationships across multiple database providers, well then, this is the repository for you.

The goal of this repository is to show how you can set up a solution to have one `DbContext` interface to multple providers. In this case, we'll be using our `VehiclesContext` to communicate with SQLite and PostgreSQL.

## Provider Switching

Provider switching happens in the host application. In this example, we're using a argument passed in at the start of the application lifetime. Since we're using the `IConfiguration` instance, we could also set this value using environment variables, JSON values, or through any other registered configuration provider.

```csharp
builder.Services.AddDbContext<VehiclesContext>(options =>
{
    var provider = config.GetValue("provider", Sqlite.Name);

    if (provider == Sqlite.Name)
    {
        options.UseSqlite(
            config.GetConnectionString(Sqlite.Name)!,
            x => x.MigrationsAssembly(Sqlite.Assembly)
        );
    }

    if (provider == Postgres.Name) {
        options.UseNpgsql(
            config.GetConnectionString(Postgres.Name)!,
            x => x.MigrationsAssembly(Postgres.Assembly)
        );
    }
});
```

A fascinating side-effect is provider switching can also happen on each request, meaning you could jump back and forth between each database provider. Not sure why you'd do that, but you could in theory.

## Commands

Commands are assumed to be running at the root of the solution directory, and you will need to adjust your paths depending on your solution and projects.

```console
> dotnet ef migrations add <Migration Name> --project ./Migrations/BoxedSoftware.Sqlite -- --provider Sqlite

> dotnet ef migrations add <Migration Name> --project ./Migrations/BoxedSoftware.Postgres -- --provider Postgres
```

## Projects, Projects, Projects

While most developers won't mind the additional projects, you will need to separate many aspects of your solution into additional projects. This sample includes a `Host`, `Models`, and one project for each database provider. That's a total of 4 projects.

## Caveats

There are caveats with this approach, the most obvious being the need to use features that are implemented in all database providers. This potentially excludes the use of `EF.Functions` in your queries. To work around these limitations you could do the following:

1. Create methods on the `DbContext` that are provider aware, switching logic based on the current provider.
2. Use stored procedures that implement database specific functionality closer to the database. (Note, stored procedures are not supported in SQLite).
3. Branching code paths in business code based on the provider, and maybe even preprocessor directives and different builds of your app.

These are all techniques to help you around certain limitations of each database provider, but ultimately you want to avoid these edge cases as much as possible to allow for the least amount of differences in code.

## License

Copyright (c) 2022 Khalid Abuhakmeh

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


