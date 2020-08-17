using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TGS.Core.DataServices.Bet.Migrations
{
    public partial class initial : Migration
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
                    Id = table.Column<Guid>(nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ErrorType = table.Column<short>(type: "smallint", nullable: false),
                    ErrorDescription = table.Column<string>(type: "nvarchar(1024)", nullable: true),
                    TransactionsCount = table.Column<int>(type: "int", nullable: false),
                    ErrorsJson = table.Column<string>(type: "nvarchar(4000)", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    GameProviderConfiguration = table.Column<string>(type: "nvarchar(4000)", nullable: false)
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
                    Id = table.Column<Guid>(nullable: false),
                    ExternalRoundId = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    GameCode = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    GameType = table.Column<short>(type: "smallint", nullable: false),
                    GameProviderName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    GameProviderCode = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    PlayerCode = table.Column<string>(type: "varchar(16)", nullable: false),
                    PlayerTypes = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    PlatformType = table.Column<short>(type: "smallint", nullable: false),
                    RoundDetailId = table.Column<Guid>(nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderConfiguration = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    ClosedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalCreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    ExternalClosedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    AggregatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    ExternalAggregatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    BetAmount = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    Commission = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    GrossGamingRevenue = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    TiebetAdjustment = table.Column<decimal>(type: "decimal(18, 6)", nullable: true)
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
                    Id = table.Column<Guid>(nullable: false),
                    ExternalBetId = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    BetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalTransactionId = table.Column<string>(type: "nvarchar(128)", nullable: true),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Turnover = table.Column<decimal>(type: "decimal(18, 6)", nullable: false),
                    GrossGamingRevenue = table.Column<decimal>(type: "decimal(18, 6)", nullable: false),
                    UnsettledBets = table.Column<decimal>(type: "decimal(18, 6)", nullable: false),
                    IsClosingRound = table.Column<bool>(nullable: false),
                    ClosingRoundExplicitRequest = table.Column<bool>(nullable: true),
                    RoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferencedGameActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GameActionBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GameProviderConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameProviderConfiguration = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    RecordedOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false),
                    SentOn = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: true),
                    StoredOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    PlayerIp = table.Column<string>(type: "varchar(128)", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 6)", nullable: false),
                    GameActionGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalGameActionGroupId = table.Column<string>(type: "nvarchar(128)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18, 6)", nullable: true),
                    BetType = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", nullable: false)
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameActions_GameActions_ReferencedGameActionId",
                        column: x => x.ReferencedGameActionId,
                        principalSchema: "dbo",
                        principalTable: "GameActions",
                        principalColumn: "Id");
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
                    Id = table.Column<Guid>(nullable: false),
                    RoundId = table.Column<Guid>(nullable: false),
                    AdditionalData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundDetails", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
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

            migrationBuilder.CreateIndex(
                name: "IX_RoundDetails_RoundId",
                schema: "dbo",
                table: "RoundDetails",
                column: "RoundId",
                unique: true);
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
