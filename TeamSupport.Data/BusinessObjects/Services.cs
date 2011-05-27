using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class Service
  {
  }
  
  public partial class Services
  {

    public virtual void LoadByName(string name)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM Services WHERE Name = @Name";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Name", name);
        Fill(command);
      }
    }

    public static Service GetService(LoginUser loginUser, string name)
    {
      Services services = new Services(loginUser);
      services.LoadByName(name);
      if (services.IsEmpty)
      {
        Service service = (new Services(loginUser)).AddNewService();
        service.Name = name;
        service.Collection.Save();
        return service;
      }
      else
      {
        return services[0];
      }
    }
  }
  
}
