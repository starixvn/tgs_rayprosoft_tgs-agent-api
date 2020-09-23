using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Apis.LogApi.Migrations
{
    public partial class Initial
    {
        //InitAfterRun(migrationBuilder);
        public void InitAfterRun(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
  ALTER TABLE [dbo].[AdminLogs] DROP CONSTRAINT PK_AdminLogs;
  ALTER TABLE [dbo].[AdminLogs] ADD CONSTRAINT PK_AdminLogs PRIMARY KEY CLUSTERED (RecordedOn desc);

  ALTER TABLE [dbo].[ApiLogs] DROP CONSTRAINT PK_ApiLogs;
  ALTER TABLE [dbo].[ApiLogs] ADD CONSTRAINT PK_ApiLogs PRIMARY KEY CLUSTERED (RecordedOn desc);

  ALTER TABLE [dbo].[LogRecords] DROP CONSTRAINT PK_LogRecords;
  ALTER TABLE [dbo].[LogRecords] ADD CONSTRAINT PK_LogRecords PRIMARY KEY CLUSTERED (RecordedOn desc);
");

            var assembly = Assembly.GetExecutingAssembly();
            migrationBuilder.SqlResource(assembly, "TGS.Apis.LogApi.DataService.SqlScripts.SP.usp_ServiceLogs_InsertServiceLog.sql");
        }
    }
}
