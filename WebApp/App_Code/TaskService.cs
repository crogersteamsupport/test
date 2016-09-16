using System.Web.Script.Services;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TaskService : System.Web.Services.WebService
    {
        public TaskService()
        {

        }


        [WebMethod]
        public ReminderProxy[] GetTasks(int from, int count, bool searchAssigned, bool searchWarehouse, bool searchJunkyard)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();

            Reminders results = new Reminders(loginUser);
            results.LoadByUser(loginUser.UserID);
            return results.GetReminderProxies();
        }
    }



    //[DataContract(Namespace = "http://teamsupport.com/")]
    //public class AdvancedSearchOptions
    //{
    //    [DataMember]
    //    public int? StandardFilterID { get; set; }
    //    [DataMember]
    //    public bool? Tickets { get; set; }
    //    [DataMember]
    //    public bool? KnowledgeBase { get; set; }
    //    [DataMember]
    //    public bool? Wikis { get; set; }
    //    [DataMember]
    //    public bool? Notes { get; set; }
    //    [DataMember]
    //    public bool? ProductVersions { get; set; }
    //    [DataMember]
    //    public bool? WaterCooler { get; set; }
    //    [DataMember]
    //    public AutoFieldItem[] Fields { get; set; }
    //    [DataMember]
    //    public SearchCustomFilterProxy[] CustomFilters { get; set; }
    //    [DataMember]
    //    public SearchSorterProxy[] Sorters { get; set; }
    //}

}