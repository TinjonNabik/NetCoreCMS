﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCMS.Framework.Modules.Widgets;
using NetCoreCMS.Framework.Core.Services;
using Microsoft.AspNetCore.Routing;
using NetCoreCMS.Framework.Core.Data;
using NetCoreCMS.Framework.Core.Models;

namespace NetCoreCMS.Framework.Modules
{
    public interface IModule
    {
        /// <summary>
        /// Unique Module ID
        /// </summary>
        string ModuleId { get; set; }
        /// <summary>
        /// Description: Module name must be same as module folder name.
        /// </summary>
        //string ModuleName { get; set; }
        /// <summary>
        /// Module title for display
        /// </summary>
        string ModuleTitle { get; set; }        
        string Author { get; set; }
        string Email { get; set; }        
        string Website { get; set; }
        string DemoUrl { get; set; }
        string ManualUrl { get; set; }
        bool AntiForgery { get; set; }
        string Description { get; set; }
        string Version { get; set; }
        string MinNccVersion { get; set; }
        string MaxNccVersion { get; set; }
        string Category { get; set; }
        List<NccModuleDependency> Dependencies { get; set; }        
        Assembly Assembly { get; set; }
        string SortName { get; set; }
        string Path { get; set; }
        string Folder { get; set; }
        int ModuleStatus { get; set; }
        List<Widget> Widgets { get; set; }
        
        void Init(IServiceCollection services);
        void RegisterRoute(IRouteBuilder routes);
        bool Install(NccSettingsService settingsService, Func<NccDbQueryText, string> executeQuery);
        bool Uninstall(NccSettingsService settingsService, Func<NccDbQueryText, string> executeQuery);
        bool Update(NccSettingsService settingsService, Func<NccDbQueryText, string> executeQuery);
        bool Activate();
        bool Inactivate();
    }
}
