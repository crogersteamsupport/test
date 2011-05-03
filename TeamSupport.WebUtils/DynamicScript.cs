using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace TeamSupport.WebUtils
{
  public class DynamicScript
  {
    private StringBuilder _builder;

    #region Properties

    private Page _page;
    public Page Page
    {
      get { return _page; }
      set { _page = value; }
    }

    private string _name;
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    #endregion

    public DynamicScript(Page page, string name)
    {
      _page = page;
      _name = name;
      _builder = new StringBuilder();
    }


    private static string GetFunction(string name, string script)
    {
      return "function " + name + "(){" + script + "Sys.Application.remove_load(" + name + ");}";
    }
    
    private static string GetLoad(string name)
    {
      return "Sys.Application.add_load(" + name + ");";
    }

    public void Add(string text)
    {
      _builder.Append(text);
    }

    public void Execute()
    {
      ExecuteScript(_page, _name, _builder.ToString());
    }

    public static void ExecuteScript(Page page, string name, string script)
    {
      ScriptManager.RegisterClientScriptBlock(page, page.GetType(), name+"_function", GetFunction(name, script), true);
      ScriptManager.RegisterStartupScript(page, page.GetType(), name, GetLoad(name), true);
     
    }


  }
}
