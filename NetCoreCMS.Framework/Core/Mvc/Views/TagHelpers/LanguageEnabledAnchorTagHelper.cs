﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;

using NetCoreCMS.Framework.i18n;
using NetCoreCMS.Framework.Setup;
using NetCoreCMS.Framework.Utility;

using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;


namespace NetCoreCMS.Framework.Core.Mvc.Views.TagHelpers
{


    [HtmlTargetElement("a")]
    [HtmlTargetElement("a", Attributes = "asp-controller")]
    [HtmlTargetElement("a", Attributes = "asp-action")]
    [HtmlTargetElement("a", Attributes = "asp-route-id")]
    public class LanguageEnabledAnchorTagHelper : TagHelper
    {
        IHttpContextAccessor _httpContextAccessor;
        public override int Order { get; } = int.MaxValue;

        [HtmlAttributeName("asp-for")]
        public string For { get; set; }        
        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }        
        [HtmlAttributeName("href")]
        public string Href { get; set; }

        public LanguageEnabledAnchorTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
            TagHelperAttribute href;

            if (GlobalConfig.WebSite != null && GlobalConfig.WebSite.IsMultiLangual)
            {
                var lang = GetCurrentLanguage();
                if (context.AllAttributes["asp-action"] != null && context.AllAttributes["asp-controller"] != null)
                {
                    output.TagName = "a";                    
                    output.Attributes.TryGetAttribute("href", out href);

                    if (href != null)
                    {
                        output.Attributes.Remove(href);
                    }

                    var finalUrl = "/" + lang + "/" + Controller + "/" + Action;
                    var req = _httpContextAccessor.HttpContext.Request;

                    var queryString = GetQueryString(context);

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        finalUrl += "/?" + queryString;
                    }

                    output.Attributes.Add(new TagHelperAttribute("href", finalUrl));
                }
                else
                {
                    if (Href != null && !IsExternalUrl(Href))
                    {
                        bool langFound = false;
                        foreach (var item in SupportedCultures.Cultures)
                        {
                            if(Href.StartsWith("/"+item.TwoLetterISOLanguageName) || Href.StartsWith(item.TwoLetterISOLanguageName))
                            {
                                langFound = true;
                                break;
                            }
                        }

                        if(langFound == false)
                        {
                            if (!Href.StartsWith("/"))
                            {
                                Href = "/" + Href;
                            }   
                            output.Attributes.SetAttribute("href", "/" + lang + Href);
                        }
                    }
                }
            }

            output.Attributes.TryGetAttribute("href", out href);
            if (href == null)
            {
                output.Attributes.Add(new TagHelperAttribute("href", Href??""));
            }
        }

        private string GetQueryString(TagHelperContext context)
        {
            var queryString = "";
            foreach (var item in context.AllAttributes)
            {
                if (item.Name.StartsWith("asp-route"))
                {
                    var parts = item.Name.Split("-".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if(parts.Length >= 3)
                    {
                        queryString += parts[2] + "=" + item.Value + "&";
                    }
                }                
            }

            if(queryString.EndsWith("&")){
                queryString = queryString.Remove(queryString.Length - 1);
            }
            return queryString;
        }

        private string GetCurrentLanguage()
        {
            var lang = "";

            if (GlobalConfig.WebSite != null && GlobalConfig.WebSite.IsMultiLangual)
            {
                lang = _httpContextAccessor.HttpContext.GetRouteValue("lang") as string;

                if (string.IsNullOrEmpty(lang))
                {
                    var feature = _httpContextAccessor.HttpContext?.Features?.Get<IRequestCultureFeature>();
                    lang = feature?.RequestCulture.Culture.TwoLetterISOLanguageName;                    
                }

                if (string.IsNullOrEmpty(lang))
                {
                    lang = SetupHelper.Language;
                }

                if (string.IsNullOrEmpty(lang))
                {
                    lang = "en";
                }
            }
            else
            {
                lang = SetupHelper.Language;
            }
            
            return lang;            
        }

        private static int GetMaxLength(IReadOnlyList<object> validatorMetadata)
        {
            for (var i = 0; i < validatorMetadata.Count; i++)
            {
                if (validatorMetadata[i] is StringLengthAttribute stringLengthAttribute && stringLengthAttribute.MaximumLength > 0)
                {
                    return stringLengthAttribute.MaximumLength;
                }

                if (validatorMetadata[i] is MaxLengthAttribute maxLengthAttribute && maxLengthAttribute.Length > 0)
                {
                    return maxLengthAttribute.Length;
                }
            }
            return 0;
        }

        private static bool IsExternalUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return reg.IsMatch(url);
            }
            return false;
        }
    }

}
