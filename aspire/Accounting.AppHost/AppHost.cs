var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Accounting_Web>("accounting-web");

builder.Build().Run();
