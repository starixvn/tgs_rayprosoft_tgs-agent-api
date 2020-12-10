using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Bet.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "GameActionBatches",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ErrorType = table.Column<short>(type: "smallint", nullable: false),
                    ErrorDescription = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    TransactionsCount = table.Column<int>(type: "int", nullable: false),
                    ErrorsJson = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameActionBatches", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalRoundId = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    GameType = table.Column<byte>(type: "tinyint", nullable: false),
                    GameProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    GameProviderCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    PlayerType = table.Column<byte>(type: "tinyint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    PlatformType = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ClosedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResellerCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    LicenseeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseeCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandCode = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    ExternalCreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExternalClosedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    BetAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    Commission = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    WinLoss = table.Column<decimal>(type: "decimal(18,6)", nullable: true),
                    TieBetAdjustment = table.Column<decimal>(type: "decimal(18,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "GameActions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Turnover = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    WinLoss = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UnsettledBets = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    BetType = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    ExternalBetId = table.Column<string>(type: "nvarchar(64)", nullable: true),
                    ExternalGameActionGroupId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    ReferencedGameActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GameActionBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsClosingRound = table.Column<bool>(type: "bit", nullable: false),
                    ClosingRoundExplicitRequest = table.Column<bool>(type: "bit", nullable: true),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    PlayerIp = table.Column<string>(type: "varchar(128)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoredOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameActions", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_GameActions_GameActionBatches_GameActionBatchId",
                        column: x => x.GameActionBatchId,
                        principalSchema: "dbo",
                        principalTable: "GameActionBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameActions_GameActions_ReferencedGameActionId",
                        column: x => x.ReferencedGameActionId,
                        principalSchema: "dbo",
                        principalTable: "GameActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameActions_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalSchema: "dbo",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoundDetails",
                schema: "dbo",
                columns: table => new
                {
                    RoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundDetails", x => x.RoundId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_RoundDetails_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalSchema: "dbo",
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameActions_GameActionBatchId",
                schema: "dbo",
                table: "GameActions",
                column: "GameActionBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_GameActions_ReferencedGameActionId",
                schema: "dbo",
                table: "GameActions",
                column: "ReferencedGameActionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameActions_RoundId",
                schema: "dbo",
                table: "GameActions",
                column: "RoundId");

            InitAfterRun(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameActions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RoundDetails",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "GameActionBatches",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Rounds",
                schema: "dbo");
        }
    }
}
