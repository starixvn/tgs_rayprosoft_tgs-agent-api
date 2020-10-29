using System;
using System.Threading.Tasks;
using TGS.Apis.BackendOffice.DataService.Models;
using TGS.Apis.BackendOffice.Services;
using TGS.Core.Infrastructures.Json;

namespace TGS.Apis.BackendOffice.DataService.DataInitializers
{
    public partial class DataInitializer
    {
        private Task GenerateSettingsAsync()
        {
            var configurations = new[]
            {
                CreateSetting(SettingConst.Menus, _jsonSerialization.SerializeToString(GetMenuSettings()), "For menus used")
            };

            return _repository.BulkInsertAsync(configurations);
        }

        private readonly IJsonSerializationProvider _jsonSerialization = new JsonSerializationProvider();


        private MenuSetting[] GetMenuSettings()
        {
            var menus = new[]
            {
                new MenuSetting("Platform")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "Announcement")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("View"),
                                new MenuSetting("Verify")
                            }
                        },
                        new MenuSetting( "StationLetter")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("View"),
                                new MenuSetting("Verify")
                            }
                        }
                    }
                },
                new MenuSetting("PlayerManagement")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "PlayerList")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View"),
                                new MenuSetting("Update")
                            }
                        }
                    }
                },
                new MenuSetting("AdminMenagement")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "Currency")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting( "Language")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting( "Whitelist")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("View")
                            }
                        },
                    }
                },
                new MenuSetting("Log")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "AdminLog")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting( "ServiceLog")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting( "ApiLog")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                    }
                },
                new MenuSetting("Report")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "OverReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        },
                        new MenuSetting( "GameProviderReport")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        }
                    }
                },
                new MenuSetting("Game")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "GameProviders")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("View")
                            }
                        }
                    }
                },
                new MenuSetting("Agent")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "Resellers")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Brands")
                            }
                        }
                    }
                },
                new MenuSetting("System")
                {
                    SubMenus = new[]
                    {
                        new MenuSetting( "Admins")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update")
                            }
                        },
                        new MenuSetting( "Roles")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete"),
                                new MenuSetting("Copy")
                            }
                        },
                        new MenuSetting( "Departments")
                        {
                            SubMenus = new[]
                            {
                                new MenuSetting("Create"),
                                new MenuSetting("Update"),
                                new MenuSetting("Delete")
                            }
                        }
                    }
                }
            };

            return menus;
        }

        private static Setting CreateSetting(string key, string value, string description)
        {
            return new Setting
            {
                Key = key,
                Value = value,
                Description = description,
                CreatedOn = DateTimeOffset.Now,
                CreatedByUsername = AdminUsername,
                UpdatedOn = DateTimeOffset.Now,
                UpdatedByUsername = AdminUsername
            };
        }
    }
}