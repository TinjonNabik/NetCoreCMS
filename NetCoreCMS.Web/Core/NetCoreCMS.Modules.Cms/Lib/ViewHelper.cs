﻿using NetCoreCMS.Framework.Core.Models;
using NetCoreCMS.Framework.i18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NetCoreCMS.Modules.Cms.Lib
{
    public class ViewHelper
    {
        public static NccPostDetails GetCurrentLanguagePostDetails(NccPost post, string language, bool isRequireContentSummary = false)
        {
            if(post != null)
            {
                var postDetails = post.PostDetails.Where(x => x.Language == language).FirstOrDefault();
                
                if (postDetails != null && isRequireContentSummary)
                {
                    postDetails.Content = Regex.Replace(postDetails.Content, "<[^>]*>", string.Empty);
                    postDetails.Content = Regex.Replace(postDetails.Content, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

                    if (postDetails.Content.Length > 300)
                    {
                        postDetails.Content = postDetails.Content.Substring(0, 300);
                    }
                }
                return postDetails;
            }
            return null;
        }
    }
}
