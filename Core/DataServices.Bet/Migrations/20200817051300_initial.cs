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
                    ExternalId = table.Column<string>(nullable: true),
                    ErrorType = table.Column<int>(nullable: false),
                    ErrorDescription = table.Column<string>(nullable: true),
                    TransactionsCount = table.Column<int>(nullable: false),
                    ErrorsJson = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    GameProviderConfiguration = table.Column<string>(nullable: true)
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
                    ExternalRoundId = table.Column<string>(nullable: true),
                    WalletId = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    GameName = table.Column<string>(nullable: true),
                    GameCode = table.Column<string>(nullable: true),
                    GameProviderName = table.Column<string>(nullable: true),
                    GameProviderCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AdditionalData = table.Column<string>(nullable: true),
                    GameProviderConfigurationId = table.Column<Guid>(nullable: false),
                    GameProviderConfiguration = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ClosedOn = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    BrandId = table.Column<Guid>(nullable: true),
                    ExternalCreatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ExternalClosedOn = table.Column<DateTimeOffset>(nullable: true),
                    AggregatedOn = table.Column<DateTimeOffset>(nullable: true),
                    ExternalAggregatedOn = table.Column<DateTimeOffset>(nullable: true),
                    BetAmount = table.Column<decimal>(nullable: true),
                    Commission = table.Column<decimal>(nullable: true),
                    GrossGamingRevenue = table.Column<decimal>(nullable: true),
                    TiebetAdjustment = table.Column<decimal>(nullable: true),
                    JackpotContributionConvertedAmount = table.Column<decimal>(nullable: true),
                    SessionId = table.Column<Guid>(nullable: true)
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
                    ExternalBetId = table.Column<string>(nullable: true),
                    BetId = table.Column<Guid>(nullable: true),
                    ExternalTransactionId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Turnover = table.Column<decimal>(nullable: false),
                    GrossGamingRevenue = table.Column<decimal>(nullable: false),
                    UnsettledBets = table.Column<decimal>(nullable: false),
                    IsClosingRound = table.Column<bool>(nullable: false),
                    ClosingRoundExplicitRequest = table.Column<bool>(nullable: true),
                    RoundId = table.Column<Guid>(nullable: false),
                    WalletId = table.Column<Guid>(nullable: false),
                    TokenId = table.Column<Guid>(nullable: true),
                    ReferencedGameActionId = table.Column<Guid>(nullable: true),
                    GameActionBatchId = table.Column<Guid>(nullable: true),
                    GameProviderConfigurationId = table.Column<Guid>(nullable: false),
                    GameProviderConfiguration = table.Column<string>(nullable: true),
                    RecordedOn = table.Column<DateTimeOffset>(nullable: false),
                    SentOn = table.Column<DateTimeOffset>(nullable: true),
                    PlayerIp = table.Column<string>(nullable: true),
                    BrandId = table.Column<Guid>(nullable: true),
                    Amount = table.Column<decimal>(nullable: true),
                    GameActionGroupId = table.Column<Guid>(nullable: true),
                    ExternalGameActionGroupId = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(nullable: true),
                    BetType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameActions",
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
