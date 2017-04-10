﻿/*
*Author: Xonaki
*Website: http://xonaki.com
*Copyright (c) xonaki.com
*License: BSD (3 Clause)
*/

using NetCoreCMS.Framework.Core.Mvc.Models;
using System.ComponentModel.DataAnnotations;

namespace NetCoreCMS.Framework.Core.Models
{
    public class NccWebSite : BaseModel
    {
        [Required]
        public string SiteTitle { get; set; }
        public string Tagline { get; set; }
        public string FaviconUrl { get; set; }
        public string SiteLogoUrl { get; set; }
        public string DomainName { get; set; }
        public string EmailAddress { get; set; }
        public bool   AllowRegistration { get; set; }
        public string NewUserRole { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string Language { get; set; }
        public string Copyrights { get; set; }
        public string PrivacyPolicyUrl { get; set; }
        public string TermsAndConditionsUrl { get; set; }
    }
}