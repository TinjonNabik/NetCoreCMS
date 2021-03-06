﻿using NetCoreCMS.Framework.i18n;
using NetCoreCMS.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreCMS.Framework.Modules.Widgets
{
    public abstract class Widget
    {
        public Widget(string widgetId, string title, string description, string footer, bool isDefault = true)
        {
            WidgetId = widgetId;
            Title = title;
            Description = description;
            Footer = footer;
            DisplayTitle = "";
            IsDefault = isDefault;
        }

        public bool IsDefault { get; }
        public string WidgetId { get; }
        public string Title { get; }
        public string DisplayTitle { get; set; }
        public string Language { get; set; }
        public string Description { get; }
        public string Footer { get; set; }
        public string ConfigJson { get; }
        public string ConfigHtml { get; set; }
        private string ConfigPrefix { get; set; }
        private string ConfigSuffix { get; set; }
        public long WebSiteWidgetId { get; set; }
        public string ViewFileName { get; set; }
        public string ConfigViewFileName { get; set; }

        public abstract void Init(long websiteWidgetId);
        public abstract string RenderBody();
        public string RenderConfig()
        {
            string titleInput = @"
                                <div class='form-group'>
                                    <label class='col-sm-3 control-label'>Title</label>
                                    <div class='col-sm-9'>
                                        <input type = 'text' class='form-control' id='title' name='title' value='' placeholder='Enter Title'>
                                    </div>
                                </div>";
            string footerInput = @"
                                <div class='form-group'>
                                    <label class='col-sm-3 control-label'>Footer</label>
                                    <div class='col-sm-9'>
                                        <input type = 'text' class='form-control' id='footer' name='footer' value='' placeholder='Enter Footer'>
                                    </div>
                                </div>";
            if (IsDefault == false)
            {
                titleInput = "";
                footerInput = "";
            }
            var culterList = SupportedCultures.Cultures.ToList();

            var langOptions = "";
            foreach (var item in culterList)
            {
                langOptions += "<option value='" + item.TwoLetterISOLanguageName + "'>" + item.DisplayName + "</option>";
            }
            var languageInput = "";
            if (GlobalConfig.WebSite.IsMultiLangual == true)
            {
                languageInput = @"<div class='form-group'>
                                    <label class='col-sm-3 control-label'>Language</label>
                                    <div class='col-sm-9'>
                                        <select class='form-control' id='language' name='language'>
	                                        <option value=''>All</option>
	                                        <!--<option value='en'>English</option>
	                                        <option value='bd'>Bangla</option>-->
                                            " + langOptions + @"
                                        </select>
                                        <!--<select name='culture' asp-for='@requestCulture.RequestCulture.UICulture.Name' asp-items='cultureItems'></select>-->
                                    </div>
                                </div>";
            }

            ConfigPrefix = @"
                            <form id='configForm_" + WebSiteWidgetId + @"' class='form-horizontal'>
                                " + languageInput + @"
                                <div>";
            ConfigSuffix = @"       
                                </div>
                                <div class='form-group'>
                                    <div class='col-sm-offset-3 col-sm-9'>
                                        <input type='button' class='btn btn-default' id='saveConfig_" + WebSiteWidgetId + @"' value='Save' />
                                    </div>
                                </div>
                            </form>

                            <script>
                                $(document).ready(function () {
                                    $('#saveConfig_" + WebSiteWidgetId + @"').on('click', function (evnt) {
                                        var formJson = $('#configForm_" + WebSiteWidgetId + @"').serializeObject();
                                        
                                        //NccUtil.Log(formJson);
                                        var data = JSON.stringify(formJson);
                                        //NccUtil.Log(data);

                                        $.ajax({
                                            url: '/CmsWidget/SaveConfig',
                                            method: 'POST',
                                            data: { webSiteWidgetId: " + WebSiteWidgetId + @", data: data},
                                            success: function (rsp) {
                                                if (rsp.isSuccess) {
                                                    NccAlert.ShowSuccess(rsp.message);
                                                }
                                                else {
                                                    NccAlert.ShowError(rsp.message);
                                                }
                                            },
                                            error: function (rsp) {
                                                NccAlert.ShowError('Error! Try to save again.');
                                            }
                                        });
                                    });

                                    setTimeout(function () {
                                        $.ajax({
                                            url: '/CmsWidget/GetConfig',
                                            method: 'GET',
                                            data: {websiteWidgetId:  " + WebSiteWidgetId + @"},
                                            success: function (rsp) {
                                                if (rsp.isSuccess) {
                                                    if(rsp.data != ''){
                                                        var data = JSON.parse(rsp.data);                                                        
                                                        var dataArr = NccUtil.JsonToArray(data);
                                                        
                                                        for (var key in dataArr) {
                                                            var elem = $('#configForm_" + WebSiteWidgetId + @" [name='+ key + ']'); 
                                                            //console.log(elem[0].tagName+'-'+elem[0].type);
                                                            $(elem).val(dataArr[key]);
                                                        }                                                        
                                                    }
                                                }
                                                else {
                                                    NccAlert.ShowError(rsp.message);
                                                }
                                            },
                                            error: function (rsp) {
                                                //NccAlert.ShowError('Error! Try to load again.');
                                            }
                                        });
                                    }, 100);
                                });
                            </script>
                            ";

            return ConfigPrefix + titleInput + ConfigHtml + footerInput + ConfigSuffix;
        }
    }
}
