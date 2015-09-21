using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamSupport.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DataRecovery
{
	public partial class Form1 : Form
	{

		private Logs _logs;
		private string _importID;

		public Form1()
		{
			InitializeComponent();

			
		}

		private LoginUser GetCorrupteLoginUser()
		{ 
		   return new LoginUser("Data Source=10.42.42.105; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -1, -1, null);
		}

		private LoginUser GetGoodLoginUser()
		{
			return new LoginUser("Data Source=10.42.42.101; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -1, -1, null);
		}


		private int GetNextOrg()
		{
		  return SqlExecutor.ExecuteInt(GetCorrupteLoginUser(), "select top 1 OrganizationID from OrgMoveEvent where HasExecuted = 0 order by DateCreated");
      }



		private void button1_Click(object sender, EventArgs e)
		{
		   int orgID = GetNextOrg();

			while (orgID > -1)
			{ 
				if (orgID < 0) return;
				_importID = orgID.ToString() + "-" + new Guid();
				_logs = new Logs(orgID.ToString() + " - Org.txt");
        RecoverCompanies(orgID);
        RecoverContacts(orgID);
        RecoverProducts(orgID);
        RecoverAssets(orgID);


        //Reocover Actiosn --recover actions that had previosly existing tickets.

        //RecoverTickets(orgID);
          // the following are handled in the ticket move
                  //RecoverActions(orgID);
                  //RecoverTicketCustomValues(orgID);
                  //RecoverTicketRelationships(orgID);
                  //RecoverAssetTickets(orgID);
                  //RecoverContactTickets(orgID);
                  //RecoverOrganizationTickets(orgID);

				SaveOrg(orgID, "Success");
			}
		}

		private void SaveOrg(int orgID, string result)
		{
			SqlExecutor.ExecuteNonQuery(GetCorrupteLoginUser(), "UPDATE OrgMoveEvent SET Result = '"+result+"', HasExecuted = 1");
		}

		private void RecoverProducts(int orgID)
		{ 
		// check corrupt db for different products,if so craete the new products, but do not use ID's
		  Products badProducts = new Products(GetCorrupteLoginUser());
		  badProducts.LoadByOrganizationID(orgID);

		  Products goodProducts = new Products(GetGoodLoginUser());
		  goodProducts.LoadByOrganizationID(orgID);

		  foreach (Product badProduct in badProducts)
		  {
			  Product goodProduct = goodProducts.FindByName(badProduct.Name);
			  if (goodProduct == null)
			  { 
			    goodProduct = (new Products(GetGoodLoginUser())).AddNewProduct();
				 goodProduct.Name = badProduct.Name;
				 goodProduct.DateCreated = badProduct.DateCreated;
				 goodProduct.CreatorID = badProduct.CreatorID;
				 goodProduct.OrganizationID = orgID;
				 goodProduct.ImportID = _importID;
         goodProduct.Collection.Save();
			  }
		  }
		}

    private void RecoverCompanies(int orgID)
    { 
    
    }

    private void RecoverContacts(int orgID)
    {

    }
    private void RecoverAssets(int orgID)
    {

    }
    
  }
}
