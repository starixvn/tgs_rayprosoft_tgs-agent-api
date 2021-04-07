using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Services;

namespace TGS.Core.DataServices.Tgs.DataInitializers
{
    partial class DataInitializer
    {
        private Task GenerateSettingsAsync()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false
            };

            var commonSettings = new[]
            {
                CreateSetting(SettingConsts.Menus,
                    _jsonSerializationProvider.SerializeToString(GetMenuSettings(), options),
                    "For menus used"),
                CreateSetting(SettingConsts.SuperLicenseeCodes,
                    _jsonSerializationProvider.SerializeToString(new[] {"TGS"}, options),
                    "Super Licensee Codes")
            };

            var gameProviderSettings = _gameProviders.Select(gp =>
            {
                var configuration = gp.GetConfiguration();
                return CreateSetting(SettingConsts.GameProviderConfigurations.Template.Args(gp.Code),
                    _jsonSerializationProvider.SerializeToString(configuration, configuration.GetType(), options),
                    $"JSON based configuration of {gp.Code}");
            }).ToArray();

            var settings = commonSettings.Concat(gameProviderSettings).ToArray();

            return _repository.BulkInsertAsync(settings);
        }

        private static Setting CreateSetting(string key, string value, string description) =>
            new()
            {
                Key = key,
                Value = value,
                Description = description,
                CreatedByUsername = AdminUsername,
                UpdatedByUsername = AdminUsername,
                CreatedOn = Now,
                UpdatedOn = Now
            };

        private static MenuSetting[] GetMenuSettings()
        {
            var menus = new[]
            {
                new MenuSetting("Platform")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("Announcement")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("View"),
                                new MenuSetting("Verify")
                            }
                        },
                        new MenuSetting("StationLetter")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("View"),
                                new MenuSetting("Verify")
                            }
                        },
                        new MenuSetting("VipLevel")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("View"),
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("PlayerManagement")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("PlayerList")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View"),
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("AgentManagement")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("AgentList")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View"),
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("AdminManagement")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("Currency")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("Language")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("Whitelist")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("View")
                            }
                        }
                    }
                },
                new MenuSetting("Game")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("Providers")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting("BrandProviders")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting("BrandGames")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("Report")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("MarketReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("OrderReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("AgentContributionReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("PlayerContributionReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("AgentBonusReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("VipReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        }
                    }
                },
                new MenuSetting("Merchant")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("Resellers")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting("Licensees")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting("Brands")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("System")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("Admins")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting("Roles")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("Copy")
                            }
                        },
                        new MenuSetting("Departments")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete")
                            }
                        },
                        new MenuSetting("BrandPayments")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View"),
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("SystemAdmin")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("Currency")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting("Settings")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("Log")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting("AdminLog")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("ApiLog")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting("Log")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                    }
                },
            };

            return menus;
        }
    }
}