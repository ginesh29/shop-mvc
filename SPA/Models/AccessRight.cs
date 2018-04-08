using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SPA.Models
{
    public class AccessRight
    {
    }
    public class AccessMenu
    {
        public long MainTabId { get; set;}
        public string TabName { get; set; }
        public string TabId { get; set; }
        public string Link { get; set; }
        public string Condition { get; set; }
        public string ConditionValue { get; set; }
    }
    public class AccessSubMenu
    {
        public long SubId { get; set; }
        public string SubTabName { get; set; }
        public string SubTabId { get; set; }
        public string SubLink { get; set; }
        public string Condition { get; set; }
        public string ConditionValue { get; set; }
        public string Link { get; set; }
        public string TabLink { get; set; }
        public string Group { get; set;}
    }
    public class ConditionList
    {
        public string Condition { get; set; }
        public string ConditionValue { get; set; }
    }
    public class TabInfo
    {
        public string MainTab { get; set; }
        public long MainTabId { get; set; }
        public string SubTab { get; set; }
        public long SubTabId { get; set; }
        public string RoleName { get; set; }
        public long RoleId { get; set; }
        public string OwnAnyStatus { get; set;}
        public long? AccesstoMenu { get; set; }
        public long? AccessToSub { get; set; }
        public string AccessToData { get; set;}
        public string insertAccess { get; set; }
        public string DeleteAccess { get; set; }
        public string UpdateAccess { get; set; }
        public long? Schlid { get; set;}
        public int? Display_Status { get; set;}
        public string Condition { get; set; }
        public string ConditionValue { get; set;}
        public string SubCondition { get; set; }
        public string SubConditionValue { get; set; }
        public string CheckCondition { get; set;}
        public string CheckSubCondition { get; set;}
    }
    public class AccessType
    {
        public string AccessName { get; set;}
        public string AccessValue1 { get; set; }
        public string AccessValue2 { get; set; }
    }
    public class AddPermission
    {
        public List<long> SelectedRoleId { get; set;}
        public List<long> UserId { get; set; }
    }
    public class Redirection
    {
        public string AccessTab { get; set;}
        public string AccessLink { get; set;}
    }
    public class getAccess
    {
        public string TabName { get; set; }
        public string subTabName { get; set; }
        public string FlowStatus { get; set; }
        public string Insert { get; set; }
        public string update { get; set; }
        public string Delete { get; set; }
        public string Display { get; set; }
    }
}