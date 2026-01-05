using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// DbMigrator  
var dbMigrator  = builder.AddProject<Projects.Accounting_DbMigrator>("DbMigrator")
        .WithReplicas(1);

builder.AddProject<Projects.Accounting_Web>("accounting-web").WithReference(dbMigrator);

builder.Build().Run();
