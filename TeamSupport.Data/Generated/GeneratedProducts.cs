using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Product : BaseItem
  {
    private Products _products;
    
    public Product(DataRow row, Products products): base(row, products)
    {
      _products = products;
    }
	
    #region Properties
    
    public Products Collection
    {
      get { return _products; }
    }
        
    
    
    
    public int ProductID
    {
      get { return (int)Row["ProductID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    

    #endregion
    
    
  }

  public partial class Products : BaseCollection, IEnumerable<Product>
  {
    public Products(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Products"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ProductID"; }
    }



    public Product this[int index]
    {
      get { return new Product(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Product product);
    partial void AfterRowInsert(Product product);
    partial void BeforeRowEdit(Product product);
    partial void AfterRowEdit(Product product);
    partial void BeforeRowDelete(int productID);
    partial void AfterRowDelete(int productID);    

    partial void BeforeDBDelete(int productID);
    partial void AfterDBDelete(int productID);    

    #endregion

    #region Public Methods

    public ProductProxy[] GetProductProxies()
    {
      List<ProductProxy> list = new List<ProductProxy>();

      foreach (Product item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int productID)
    {
      BeforeDBDelete(productID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Products] WHERE ([ProductID] = @ProductID);";
        deleteCommand.Parameters.Add("ProductID", SqlDbType.Int);
        deleteCommand.Parameters["ProductID"].Value = productID;

        BeforeRowDelete(productID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(productID);
      }
      AfterDBDelete(productID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ProductsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Products] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [Description] = @Description,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([ProductID] = @ProductID);";

		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Products] (    [OrganizationID],    [Name],    [Description],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @OrganizationID, @Name, @Description, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Products] WHERE ([ProductID] = @ProductID);";
		deleteCommand.Parameters.Add("ProductID", SqlDbType.Int);

		try
		{
		  foreach (Product product in this)
		  {
			if (product.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(product);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = product.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ProductID"].AutoIncrement = false;
			  Table.Columns["ProductID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				product.Row["ProductID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(product);
			}
			else if (product.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(product);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = product.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(product);
			}
			else if (product.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)product.Row["ProductID", DataRowVersion.Original];
			  deleteCommand.Parameters["ProductID"].Value = id;
			  BeforeRowDelete(id);
			  deleteCommand.ExecuteNonQuery();
			  AfterRowDelete(id);
			}
		  }
		  //transaction.Commit();
		}
		catch (Exception)
		{
		  //transaction.Rollback();
		  throw;
		}
		Table.AcceptChanges();
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public void BulkSave()
    {

      foreach (Product product in this)
      {
        if (product.Row.Table.Columns.Contains("CreatorID") && (int)product.Row["CreatorID"] == 0) product.Row["CreatorID"] = LoginUser.UserID;
        if (product.Row.Table.Columns.Contains("ModifierID")) product.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Product FindByProductID(int productID)
    {
      foreach (Product product in this)
      {
        if (product.ProductID == productID)
        {
          return product;
        }
      }
      return null;
    }

    public virtual Product AddNewProduct()
    {
      if (Table.Columns.Count < 1) LoadColumns("Products");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Product(row, this);
    }
    
    public virtual void LoadByProductID(int productID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ProductID], [OrganizationID], [Name], [Description], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[Products] WHERE ([ProductID] = @ProductID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ProductID", productID);
        Fill(command);
      }
    }
    
    public static Product GetProduct(LoginUser loginUser, int productID)
    {
      Products products = new Products(loginUser);
      products.LoadByProductID(productID);
      if (products.IsEmpty)
        return null;
      else
        return products[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Product> Members

    public IEnumerator<Product> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Product(row, this);
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
