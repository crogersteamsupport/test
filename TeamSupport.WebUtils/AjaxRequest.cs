using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace TeamSupport.WebUtils
{
  public class AjaxRequest
  {
    string _domainName = "";
    public string DomainName
    {
      get { return _domainName; }
      set { _domainName = value; }
    }

    string _actionObject = "";
    public string ActionObject
    {
      get { return _actionObject; }
      set { _actionObject = value; }
    }

    string _actionName = "";
    public string ActionName
    {
      get { return _actionName; }
      set { _actionName = value; }
    }

    List<string> _arguments;
    public List<string> Arguments
    {
      get { return _arguments; }
      set { _arguments = value; }
    }

    public bool IsMyDomain(string myDomainName)
    {
      return (_domainName == myDomainName || _domainName == "All" || _domainName == "Dialog");
    }
    
    public AjaxRequest(string argument)
    {
      string[] args = argument.Split(',');

      if (args.Length > 0) _domainName = args[0];
      if (args.Length > 1) _actionObject = args[1];
      if (args.Length > 2) _actionName = args[2];

      _arguments = new List<string>();

      for (int i = 3; i < args.Length; i++)
      {
        _arguments.Add(args[i]); 
      }
      
      
    }

  }
}
