using System.Reflection;
using Microsoft.EntityFrameworkCore.Migrations;
using TGS.Core.DataServices.Base.Extensions;

namespace TGS.Core.DataServices.Tgs.Migrations
{
	public partial class Initial: Migration
	{
		private static void InitAfterRun(MigrationBuilder migrationBuilder)
		{
			var assembly = Assembly.GetExecutingAssembly();

			#region Procedures

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.ProcedureSqlFormat, "player_deauthorize_by_login_record_id"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.ProcedureSqlFormat, "player_deauthorize_by_platform_type"));

			#endregion Procedures

			#region Functions

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_brand_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_brand_game_provider_configuration_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_brand_game_providers_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_brand_games_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_languages_by_external_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_languages_by_internal_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_currencies_by_external_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "brand_get_currencies_by_internal_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_currencies_by_external_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_currencies_by_internal_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_languages_by_external_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_languages_by_internal_code"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_game_provider_configuration_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat,
					"game_provider_get_games_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_game_provider_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_provider_get_sub_game_id_maps"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "localization_get_localization_by_game_provider_id"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "player_get_player_for_cache"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "player_get_player_login_records"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "wallet_get_balance"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "wallet_update_wallet_balance"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_action_get_game_actions_in_same_round"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_action_get_game_action_external_ids"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_action_get_valid_game_action_external_ids"));

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.FunctionSqlFormat, "game_action_get_game_actions_by_external_round_id"));

			#endregion Functions

			#region Views

			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.ViewSqlFormat, "vw_valid_game_actions"));

			#endregion Views

			#region Specific Indexes

			// unique index of players for lower case username, instead of alternative key
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "players"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "resellers"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "licensees"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "brands"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "payments"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "paymentMethods"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "paymentChannels"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "roles"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "users"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "games"));
			migrationBuilder.SqlResource(assembly,
				string.Format(Consts.IndexSqlFormat, "gameProviders"));

			#endregion
		}
	}
}