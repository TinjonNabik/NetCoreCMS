﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCMS.Framework.Core.Models;


namespace NetCoreCMS.Framework.Modules
{
    public interface IPlugins
    {
        string PluginsId { get; set; }
        string PluginsTitle { get; set; }
        bool AntiForgery { get; set; }
        string Author { get; set; }
        string Website { get; set; } 
        Version Version { get; set; }
        Version NetCoreCMSVersion { get; set; }
        string Description { get; set; }
        List<string> Dependencies { get; set; }
        string Category { get; set; }
        Assembly Assembly { get; set; }
        string SortName { get; set; }
        string Path { get; set; }
        NccPlugins.NccPluginsStatus Status { get; set; }

        void Init(IServiceCollection services);
        bool Install();
        bool Uninstall();
        bool Activate();
        bool Inactivate();
        void LoadPluginsInfo();
    }
}
