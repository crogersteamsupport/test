using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TeamSupport.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;
using System.Web.Security;
using System.IO;
using Microsoft.Win32;
using System.Data.OleDb;



namespace TeamSupport.Data
{
  using IdList = SortedList<string, int>;
  using ImportCustomFields = List<ImportCustomField>;

  class ImportCustomField
  {
    public ImportCustomField(string tableName, string fieldName, int tsFieldID, ReferenceType refType, int auxID)
    {
      TableName = tableName;
      FieldName = fieldName;
      TSFieldID = tsFieldID;
      RefType = refType;
      AuxID = auxID;
    }

    public int AuxID { get; set; }
    public string TableName { get; set; }
    public string FieldName { get; set; }
    public int TSFieldID { get; set; }
    public ReferenceType RefType { get; set; }
  }

  public class Importer
  {
    const int BULK_LIMIT = 1000;

    class ImportCustomInfo
    {
      public ImportCustomInfo(CustomFieldType fieldType, string listValues)
      {
        FieldType = fieldType;
        ListValues = listValues;
      }

      public CustomFieldType FieldType;
      public string ListValues;
    }

    class ImportType
    {
      public ImportType(string typeName, ReferenceType referenceType, int tsTypeID)
      {
        TypeName = typeName;
        ReferenceType = referenceType;
        TSTypeID = tsTypeID;
      }

      public string TypeName;
      public ReferenceType ReferenceType;
      public int TSTypeID;
    }

    private ImportCustomFields _importCustomFields;
    private LoginUser _loginUser;
    private User _creator;
    private DataRow _currentRow;
    private ImportLog _log;
    private int _organizationID;
    private string _fileName;
    private string[] _ticketTypeNames;

    public Importer(LoginUser loginUser, string fileName)
    {
      _creator = Users.GetUser(loginUser, loginUser.UserID);
      _loginUser = new LoginUser(loginUser.ConnectionString, -1, loginUser.OrganizationID, null);
      _fileName = fileName;
    }

    private string GetConnectionString()
    {
      return @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + _fileName + @";Extended Properties=""Excel 12.0;HDR=YES;IMEX=1""";
      //@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + _fileName + @";Extended Properties=""Excel 8.0;HDR=YES;IMEX=1""";
    }

    private DataRow ReadRow(string tableName, string keyColumnName, string value)
    {
      DataTable table = new DataTable();
      using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
      {

        string query = string.Format("SELECT * FROM [{0}$] WHERE {1} = '{2}'", tableName, keyColumnName, value);
        OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
        connection.Open();
        adapter.Fill(table);
        connection.Close();
      }

      if (table.Rows.Count < 1) return null;

      return table.Rows[0];
    }

    private DataTable ReadTable(string tableName)
    {
      DataTable table = new DataTable();
      using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
      {

        string query = "SELECT * FROM [" + tableName + "$]";
        OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
        connection.Open();
        adapter.Fill(table);
        connection.Close();
      }

      for (int i = table.Rows.Count - 1; i >= 0; i--)
      {
        DataRow row = table.Rows[i];
        if (row[0].ToString().Trim() == "") row.Delete();
      }
      table.AcceptChanges();

      return table;

    }

    private DataTable ReadDistinctValues(string tableName, string columnName)
    {
      DataTable table = new DataTable();
      using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
      {

        string query = string.Format("SELECT DISTINCT ({0}) FROM [{1}$]", columnName, tableName);
        OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
        connection.Open();
        adapter.Fill(table);
        connection.Close();
      }

      return table;
    }

    private int ReadRecordCount(string tableName)
    {
      DataTable table = new DataTable();
      using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
      {
        OleDbCommand command = new OleDbCommand();
        command.CommandText = string.Format("SELECT COUNT(*) FROM [{0}$]", tableName);
        command.CommandType = CommandType.Text;
        command.Connection = connection;
        connection.Open();
        object o = command.ExecuteScalar();
        connection.Close();

        if (o == null || o == DBNull.Value) return 0;
        else return (int)o;
      }
    }

    private DataTable ReadEmptyTable(string tableName)
    {
      DataTable table = new DataTable();
      using (OleDbConnection connection = new OleDbConnection(GetConnectionString()))
      {

        string query = "SELECT * FROM [" + tableName + "$] WHERE 0=1";
        OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
        connection.Open();
        adapter.Fill(table);
        connection.Close();
      }

      for (int i = table.Rows.Count - 1; i >= 0; i--)
      {
        DataRow row = table.Rows[i];
        if (row[0].ToString().Trim() == "") row.Delete();
      }
      table.AcceptChanges();

      return table;

    }

    private int CreateOrganization(string name)
    {
      Organization organization = (new Organizations(_loginUser)).AddNewOrganization();
      organization.Name = name;
      organization.Description = "";
      organization.Website = "";
      organization.ParentID = 1;
      organization.PortalSeats = 100;
      organization.IsApiActive = true;
      organization.IsApiEnabled = false;
      organization.IsAdvancedPortal = true;
      organization.HasPortalAccess = true;
      organization.IsBasicPortal = true;
      organization.ShowWiki = true;
      organization.WhereHeard = "";
      organization.TimeZoneID = "Central Standard Time";
      organization.IsActive = true;
      organization.IsCustomerFree = false;
      organization.BusinessDays = 62;
      organization.ExtraStorageUnits = 0;
      organization.UserSeats = 100;
      organization.ChatSeats = 100;
      organization.APIRequestLimit = 5000;
      organization.CultureName = "en-US";
      organization.ProductType = ProductType.Enterprise;
      organization.AdminOnlyCustomers = false;

      organization.Collection.Save();


      User user = (new Users(_loginUser)).AddNewUser();
      user.ActivatedOn = DateTime.UtcNow;
      user.Email = _creator.Email;
      user.FirstName = user.FirstName;
      user.InOffice = true;
      user.IsActive = true;
      user.IsChatUser = true;
      user.IsFinanceAdmin = true;
      user.IsPasswordExpired = false;
      user.IsPortalUser = true;
      user.IsSystemAdmin = true;
      user.LastActivity = DateTime.UtcNow;
      user.LastLogin = DateTime.UtcNow;
      user.LastName = _creator.LastName;
      user.LastPing = DateTime.UtcNow;
      user.MarkDeleted = false;
      user.CryptedPassword = _creator.CryptedPassword;
      user.OrganizationID = organization.OrganizationID;
      user.ReceiveTicketNotifications = true;
      user.SubscribeToNewTickets = false;
      user.Collection.Save();

      return organization.OrganizationID;
    }

    private void CreateTicketTypes()
    {
      TicketTypes ticketTypes = new TicketTypes(_loginUser);
      ticketTypes.LoadAllPositions(_organizationID);

      string[] value = GetSettingString("TicketTypes").Split(',');
      List<string> ticketTypeNames = new List<string>();

      foreach (string s in value)
      {
        string name = s.Trim();
        ticketTypeNames.Add(name);
        if (ticketTypes.FindByName(name) == null)
        {
          ticketTypes = new TicketTypes(_loginUser);
          TicketType ticketType = ticketTypes.AddNewTicketType();
          ticketType.Name = name;
          ticketType.Description = "";
          ticketType.OrganizationID = _organizationID;
          ticketType.Position = ticketTypes.GetMaxPosition(_organizationID) + 1;
          ticketTypes.Save();
          ticketTypes.LoadAllPositions(_organizationID);
        }
      }
      _ticketTypeNames = ticketTypeNames.ToArray();
    }

    public string Import(int organizationID)
    {
      using (_log = new ImportLog(Path.ChangeExtension(_fileName, ".log")))
      {

        _organizationID = organizationID;
        _loginUser = new LoginUser(_loginUser.ConnectionString, -1, organizationID, null);

        _log.AppendMessage("Import Started: " + DateTime.Now.ToString());
        _currentRow = null;

        CreateTicketTypes();

        using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
        {
          connection.Open();

          try
          {

            GC.WaitForPendingFinalizers();
            ImportUsers();
            GC.WaitForPendingFinalizers();
            ImportGroups();
            GC.WaitForPendingFinalizers();
            ImportUserGroups();
            GC.WaitForPendingFinalizers();
            ImportCustomers();
            GC.WaitForPendingFinalizers();
            ImportContacts();
            GC.WaitForPendingFinalizers();
            ImportPrimaryContacts();
            GC.WaitForPendingFinalizers();
            ImportProducts();
            GC.WaitForPendingFinalizers();
            ImportProductVersions();
            GC.WaitForPendingFinalizers();
            ImportAssets();
            GC.WaitForPendingFinalizers();
            ImportTickets();
            GC.WaitForPendingFinalizers();
            ImportActions();
            GC.WaitForPendingFinalizers();
            ImportAttachments();
            GC.WaitForPendingFinalizers();
            ImportAddresses();
            GC.WaitForPendingFinalizers();
            ImportPhoneNumbers();
            GC.WaitForPendingFinalizers();
            ImportNotes();
            GC.WaitForPendingFinalizers();
            ImportCustomerProducts();
            GC.WaitForPendingFinalizers();
            ImportCustomerTickets();
            GC.WaitForPendingFinalizers();
            ImportContactTickets();
            GC.WaitForPendingFinalizers();
            ImportWiki();
            GC.WaitForPendingFinalizers();
            ImportCustomFields();
            GC.WaitForPendingFinalizers();
          }
          catch (Exception ex)
          {
            ExceptionLogs.LogException(_loginUser, ex, "Import Module", _currentRow);
            string log = "Exception: {0}\n\nStack Trace: {1}\n\nData Row {2}\n\nImport TERMINATED!";
            _log.AppendMessage(string.Format(log, ex.Message, ex.StackTrace, DataUtils.DataRowToString(_currentRow)));
            return ex.Message;
          }
          connection.Close();
        }
        _log.AppendMessage("Import Ended: " + DateTime.Now.ToString());

        EmailPosts.DeleteImportEmails(_loginUser);
      }

      return "";

    }

    public string Import(string organizationName)
    {
      return Import(CreateOrganization(organizationName));
    }

    #region Type Import Methods

    private void CreateAllCustomFields()
    {
      foreach (string ticketTypeName in _ticketTypeNames)
      {
        CreateCustomFields(ticketTypeName, "DateClosed", true);
      }
      CreateCustomFields("Customers", "Website");
      CreateCustomFields("Contacts", "Title");
      CreateCustomFields("Users", "IsActive");
      CreateCustomFields("Groups", "Description");
      CreateCustomFields("Products", "Description");
      CreateCustomFields("Versions", "IsReleased");
      CreateCustomFields("CustomerProducts", "VersionID");
      CreateCustomFields("Assets", "Status");
    }

    private void CreateCustomFields(string tableName, string lastColumnName)
    {
      CreateCustomFields(tableName, lastColumnName, false);
    }

    private void CreateCustomFields(string tableName, string lastColumnName, bool isTicket)
    {
      TicketTypes ticketTypes = new TicketTypes(_loginUser);
      ticketTypes.LoadAllPositions(_organizationID);

      ReferenceType referenceType = ReferenceType.None;
      CustomFields customFields = new CustomFields(_loginUser);
      int auxID = -1;

      if (isTicket)
      {
        referenceType = ReferenceType.Tickets; auxID = ticketTypes.FindByName(tableName).TicketTypeID;
      }
      else
      {
        switch (tableName)
        {
          case "Customers": referenceType = ReferenceType.Organizations; auxID = -1; break;
          case "Contacts": referenceType = ReferenceType.Contacts; auxID = -1; break;
          case "Users": referenceType = ReferenceType.Users; auxID = -1; break;
          case "Products": referenceType = ReferenceType.Products; auxID = -1; break;
          case "Versions": referenceType = ReferenceType.ProductVersions; auxID = -1; break;
          case "CustomerProducts": referenceType = ReferenceType.OrganizationProducts; auxID = -1; break;
          case "Assets": referenceType = ReferenceType.Assets; auxID = -1; break;
          default:
            break;
        }
      }

      customFields.LoadByReferenceType(_organizationID, referenceType, auxID);


      DataTable table = ReadTable(tableName);
      int index = -1;
      for (int i = 0; i < table.Columns.Count; i++)
      {
        if (table.Columns[i].ColumnName == lastColumnName)
        {
          index = i + 1;
          break;
        }
      }

      for (int i = index; i < table.Columns.Count; i++)
      {
        string name = table.Columns[i].ColumnName;

        CustomField field = customFields.FindByName(name);
        if (field == null)
        {
          field = (new CustomFields(_loginUser)).AddNewCustomField();
          field.OrganizationID = _organizationID;
          field.Name = name.Trim();
          field.ApiFieldName = tableName + "_" + field.Name;
          if (auxID > -1) field.ApiFieldName = field.ApiFieldName + "_" + auxID.ToString();
          field.ApiFieldName = CustomFields.GenerateApiFieldName(field.ApiFieldName);
          field.Position = customFields.GetMaxPosition(_organizationID, referenceType, auxID);
          field.AuxID = auxID;
          field.RefType = referenceType;
          field.Description = "";
          ImportCustomInfo info = GetCustomInfo(table, table.Columns[i].ColumnName);
          field.FieldType = info.FieldType;
          field.ListValues = info.ListValues;
          field.Collection.Save();
          customFields.LoadByReferenceType(_organizationID, referenceType, auxID);
        }

        _importCustomFields.Add(new ImportCustomField(tableName, name, field.CustomFieldID, referenceType, auxID));
      }
    }

    private ProductVersionStatus GetVersionStatus(ProductVersionStatuses statuses, string name)
    {
      name = name.Trim();
      ProductVersionStatus result = statuses.FindByName(name);
      if (result == null)
      {
        result = (new ProductVersionStatuses(_loginUser)).AddNewProductVersionStatus();
        result.IsDiscontinued = false;
        result.IsShipping = true;
        result.Name = name;
        result.OrganizationID = _organizationID;
        result.Position = statuses.GetMaxPosition(_organizationID) + 1;
        result.Collection.Save();
      }
      statuses.LoadAllPositions(_organizationID);
      return result;
    }

    private PhoneType GetPhoneType(PhoneTypes phoneTypes, string name)
    {
      name = name.Trim();
      PhoneType result = phoneTypes.FindByName(name);
      if (result == null)
      {
        result = (new PhoneTypes(_loginUser)).AddNewPhoneType();
        result.Name = name;
        result.OrganizationID = _organizationID;
        result.Position = phoneTypes.GetMaxPosition(_organizationID) + 1;
        result.Collection.Save();
        phoneTypes.LoadAllPositions(_organizationID);
      }
      return result;
    }

    private TicketStatus GetTicketStatus(TicketStatuses statuses, string name, TicketType ticketType)
    {
      name = name.Trim();
      TicketStatus result = statuses.FindByName(name, ticketType.TicketTypeID);
      if (result == null)
      {
        result = (new TicketStatuses(_loginUser)).AddNewTicketStatus();
        result.Name = name;
        result.Description = "";
        result.TicketTypeID = ticketType.TicketTypeID;
        result.OrganizationID = _organizationID;
        result.Position = statuses.GetMaxPosition(_organizationID) + 1;
        result.IsClosed = false;
        result.Collection.Save();

      }
      statuses.LoadAllPositions(ticketType.TicketTypeID);
      return result;
    }

    private TicketSeverity GetTicketSeverity(TicketSeverities severities, string name)
    {
      name = name.Trim();
      TicketSeverity result = severities.FindByName(name);
      if (result == null)
      {
        result = (new TicketSeverities(_loginUser)).AddNewTicketSeverity();
        result.Name = name;
        result.Description = "";
        result.OrganizationID = _organizationID;
        result.Position = severities.GetMaxPosition(_organizationID) + 1;
        result.Collection.Save();

      }
      severities.LoadAllPositions(_organizationID);
      return result;
    }

    private SystemActionType GetSystemActionTypeID(string name)
    {
      SystemActionType result = SystemActionType.Custom;
      switch (name.ToLower().Trim())
      {
        case "description": result = SystemActionType.Description; break;
        case "resolution": result = SystemActionType.Resolution; break;
        case "pingupdate": result = SystemActionType.PingUpdate; break;
        case "email": result = SystemActionType.Email; break;
        default:
          break;
      }
      return result;
    }

    private int? GetActionTypeID(ActionTypes actionTypes, string name)
    {
      name = name.Trim();
      if (GetSystemActionTypeID(name) != SystemActionType.Custom) return null;

      ActionType actionType = actionTypes.FindByName(name);

      if (actionType == null)
      {
        actionTypes = new ActionTypes(_loginUser);
        actionType = actionTypes.AddNewActionType();
        actionType.Name = name;
        actionType.Description = "";
        actionType.IsTimed = true;
        actionType.OrganizationID = _organizationID;
        actionType.Position = actionTypes.GetMaxPosition(_organizationID) + 1;
        actionTypes.Save();
        actionTypes.LoadAllPositions(_organizationID);
      }

      return actionType.ActionTypeID;
    }

    #endregion

    #region Utility Methods
    private static void SetTypeGuessRowsForExcelImport()
    {
      RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Jet\4.0\Engines\Excel", true);
      if (key == null)
      {
        key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Jet\4.0\Engines\Excel", true);
      }

      if (key != null)
      {
        key.SetValue("TypeGuessRows", 0);
        key.SetValue("ImportMixedTypes", "Text");
      }

    }

    private string GetListToCommaString(List<string> list)
    {
      StringBuilder builder = new StringBuilder();
      foreach (string s in list)
      {
        if (builder.Length > 0) builder.Append("|");
        builder.Append(s);
      }

      return builder.ToString();
    }

    private ImportCustomInfo GetCustomInfo(DataTable table, string columnName)
    {

      // test for date time
      bool flag = true;
      bool hasData = false;
      int count = 0;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        if (count++ > 100) break;
        string value = row[columnName].ToString().Trim();
        if (value == "") continue;
        try
        {
          hasData = true;
          DateTime.Parse(value);
        }
        catch (Exception)
        {
          flag = false;
          break;
        }
      }


      if (flag && hasData)
      {
        return new ImportCustomInfo(CustomFieldType.DateTime, "");
      }


      // test for number
      flag = true;
      count = 0;
      hasData = false;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        if (count++ > 100) break;
        string value = row[columnName].ToString().Trim();
        if (value == "") continue;
        try
        {
          double.Parse(value);
          hasData = true;
        }
        catch (Exception)
        {
          flag = false;
          break;
        }
      }


      if (flag && hasData)
      {
        return new ImportCustomInfo(CustomFieldType.Number, "");
      }


      List<string> list = new List<string>();
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        string value = row[columnName].ToString().Trim();

        if (value != "" && list.IndexOf(value) < 0) list.Add(value);
        if (list.Count > 50)
        {
          list.Clear();
          return new ImportCustomInfo(CustomFieldType.Text, "");
        }
      }


      if (list.Count < 2) return new ImportCustomInfo(CustomFieldType.Text, "");

      if (list.Count == 2)
      {
        if ((list[0].ToLower().Trim() == "false" ||
            list[0].ToLower().Trim() == "true" ||
            list[0].ToLower().Trim() == "no" ||
            list[0].ToLower().Trim() == "yes")
            &&
            (list[1].ToLower().Trim() == "false" ||
            list[1].ToLower().Trim() == "true" ||
            list[1].ToLower().Trim() == "no" ||
            list[1].ToLower().Trim() == "yes"))
        {
          return new ImportCustomInfo(CustomFieldType.Boolean, "");
        }
      }

      return new ImportCustomInfo(CustomFieldType.PickList, GetListToCommaString(list));

    }

    private static string MimeType(string _fileName)
    {
      string result = "";
      switch (Path.GetExtension(_fileName).ToLower())
      {
        case ".3dm": result = "x-world/x-3dmf"; break;
        case ".3dmf": result = "x-world/x-3dmf"; break;
        case ".a": result = "application/octet-stream"; break;
        case ".aab": result = "application/x-authorware-bin"; break;
        case ".aam": result = "application/x-authorware-map"; break;
        case ".aas": result = "application/x-authorware-seg"; break;
        case ".abc": result = "text/vnd.abc"; break;
        case ".acgi": result = "text/html"; break;
        case ".afl": result = "video/animaflex"; break;
        case ".ai": result = "application/postscript"; break;
        case ".aif": result = "audio/aiff"; break;
        case ".aifc": result = "audio/aiff"; break;
        case ".aiff": result = "audio/aiff"; break;
        case ".aim": result = "application/x-aim"; break;
        case ".aip": result = "text/x-audiosoft-intra"; break;
        case ".ani": result = "application/x-navi-animation"; break;
        case ".aos": result = "application/x-nokia-9000-communicator-add-on-software"; break;
        case ".aps": result = "application/mime"; break;
        case ".arc": result = "application/octet-stream"; break;
        case ".arj": result = "application/arj"; break;
        case ".art": result = "image/x-jg"; break;
        case ".asf": result = "video/x-ms-asf"; break;
        case ".asm": result = "text/x-asm"; break;
        case ".asp": result = "text/asp"; break;
        case ".asx": result = "video/x-ms-asf"; break;
        case ".au": result = "audio/basic"; break;
        case ".avi": result = "video/avi"; break;
        case ".avs": result = "video/avs-video"; break;
        case ".bcpio": result = "application/x-bcpio"; break;
        case ".bin": result = "application/octet-stream"; break;
        case ".bm": result = "image/bmp"; break;
        case ".bmp": result = "image/bmp"; break;
        case ".boo": result = "application/book"; break;
        case ".book": result = "application/book"; break;
        case ".boz": result = "application/x-bzip2"; break;
        case ".bsh": result = "application/x-bsh"; break;
        case ".bz": result = "application/x-bzip"; break;
        case ".bz2": result = "application/x-bzip2"; break;
        case ".c": result = "text/plain"; break;
        case ".c++": result = "text/plain"; break;
        case ".cat": result = "application/vnd.ms-pki.seccat"; break;
        case ".cc": result = "text/plain"; break;
        case ".ccad": result = "application/clariscad"; break;
        case ".cco": result = "application/x-cocoa"; break;
        case ".cdf": result = "application/cdf"; break;
        case ".cer": result = "application/pkix-cert"; break;
        case ".cha": result = "application/x-chat"; break;
        case ".chat": result = "application/x-chat"; break;
        case ".class": result = "application/java"; break;
        case ".com": result = "application/octet-stream"; break;
        case ".conf": result = "text/plain"; break;
        case ".cpio": result = "application/x-cpio"; break;
        case ".cpp": result = "text/x-c"; break;
        case ".cpt": result = "application/x-cpt"; break;
        case ".crl": result = "application/pkcs-crl"; break;
        case ".crt": result = "application/pkix-cert"; break;
        case ".csh": result = "application/x-csh"; break;
        case ".css": result = "text/css"; break;
        case ".cxx": result = "text/plain"; break;
        case ".dcr": result = "application/x-director"; break;
        case ".deepv": result = "application/x-deepv"; break;
        case ".def": result = "text/plain"; break;
        case ".der": result = "application/x-x509-ca-cert"; break;
        case ".dif": result = "video/x-dv"; break;
        case ".dir": result = "application/x-director"; break;
        case ".dl": result = "video/dl"; break;
        case ".doc": result = "application/msword"; break;
        case ".dot": result = "application/msword"; break;
        case ".dp": result = "application/commonground"; break;
        case ".drw": result = "application/drafting"; break;
        case ".dump": result = "application/octet-stream"; break;
        case ".dv": result = "video/x-dv"; break;
        case ".dvi": result = "application/x-dvi"; break;
        case ".dwf": result = "model/vnd.dwf"; break;
        case ".dwg": result = "image/vnd.dwg"; break;
        case ".dxf": result = "image/vnd.dwg"; break;
        case ".dxr": result = "application/x-director"; break;
        case ".el": result = "text/x-script.elisp"; break;
        case ".elc": result = "application/x-elc"; break;
        case ".env": result = "application/x-envoy"; break;
        case ".eps": result = "application/postscript"; break;
        case ".es": result = "application/x-esrehber"; break;
        case ".etx": result = "text/x-setext"; break;
        case ".evy": result = "application/envoy"; break;
        case ".exe": result = "application/octet-stream"; break;
        case ".f": result = "text/plain"; break;
        case ".f77": result = "text/x-fortran"; break;
        case ".f90": result = "text/plain"; break;
        case ".fdf": result = "application/vnd.fdf"; break;
        case ".fif": result = "image/fif"; break;
        case ".fli": result = "video/fli"; break;
        case ".flo": result = "image/florian"; break;
        case ".flx": result = "text/vnd.fmi.flexstor"; break;
        case ".fmf": result = "video/x-atomic3d-feature"; break;
        case ".for": result = "text/x-fortran"; break;
        case ".fpx": result = "image/vnd.fpx"; break;
        case ".frl": result = "application/freeloader"; break;
        case ".funk": result = "audio/make"; break;
        case ".g": result = "text/plain"; break;
        case ".g3": result = "image/g3fax"; break;
        case ".gif": result = "image/gif"; break;
        case ".gl": result = "video/gl"; break;
        case ".gsd": result = "audio/x-gsm"; break;
        case ".gsm": result = "audio/x-gsm"; break;
        case ".gsp": result = "application/x-gsp"; break;
        case ".gss": result = "application/x-gss"; break;
        case ".gtar": result = "application/x-gtar"; break;
        case ".gz": result = "application/x-gzip"; break;
        case ".gzip": result = "application/x-gzip"; break;
        case ".h": result = "text/plain"; break;
        case ".hdf": result = "application/x-hdf"; break;
        case ".help": result = "application/x-helpfile"; break;
        case ".hgl": result = "application/vnd.hp-hpgl"; break;
        case ".hh": result = "text/plain"; break;
        case ".hlb": result = "text/x-script"; break;
        case ".hlp": result = "application/hlp"; break;
        case ".hpg": result = "application/vnd.hp-hpgl"; break;
        case ".hpgl": result = "application/vnd.hp-hpgl"; break;
        case ".hqx": result = "application/binhex"; break;
        case ".hta": result = "application/hta"; break;
        case ".htc": result = "text/x-component"; break;
        case ".htm": result = "text/html"; break;
        case ".html": result = "text/html"; break;
        case ".htmls": result = "text/html"; break;
        case ".htt": result = "text/webviewhtml"; break;
        case ".htx": result = "text/html"; break;
        case ".ice": result = "x-conference/x-cooltalk"; break;
        case ".ico": result = "image/x-icon"; break;
        case ".idc": result = "text/plain"; break;
        case ".ief": result = "image/ief"; break;
        case ".iefs": result = "image/ief"; break;
        case ".iges": result = "application/iges"; break;
        case ".igs": result = "application/iges"; break;
        case ".ima": result = "application/x-ima"; break;
        case ".imap": result = "application/x-httpd-imap"; break;
        case ".inf": result = "application/inf"; break;
        case ".ins": result = "application/x-internett-signup"; break;
        case ".ip": result = "application/x-ip2"; break;
        case ".isu": result = "video/x-isvideo"; break;
        case ".it": result = "audio/it"; break;
        case ".iv": result = "application/x-inventor"; break;
        case ".ivr": result = "i-world/i-vrml"; break;
        case ".ivy": result = "application/x-livescreen"; break;
        case ".jam": result = "audio/x-jam"; break;
        case ".jav": result = "text/plain"; break;
        case ".java": result = "text/plain"; break;
        case ".jcm": result = "application/x-java-commerce"; break;
        case ".jfif": result = "image/jpeg"; break;
        case ".jfif-tbnl": result = "image/jpeg"; break;
        case ".jpe": result = "image/jpeg"; break;
        case ".jpeg": result = "image/jpeg"; break;
        case ".jpg": result = "image/jpeg"; break;
        case ".jps": result = "image/x-jps"; break;
        case ".js": result = "application/x-javascript"; break;
        case ".jut": result = "image/jutvision"; break;
        case ".kar": result = "audio/midi"; break;
        case ".ksh": result = "application/x-ksh"; break;
        case ".la": result = "audio/nspaudio"; break;
        case ".lam": result = "audio/x-liveaudio"; break;
        case ".latex": result = "application/x-latex"; break;
        case ".lha": result = "application/octet-stream"; break;
        case ".lhx": result = "application/octet-stream"; break;
        case ".list": result = "text/plain"; break;
        case ".lma": result = "audio/nspaudio"; break;
        case ".log": result = "text/plain"; break;
        case ".lsp": result = "application/x-lisp"; break;
        case ".lst": result = "text/plain"; break;
        case ".lsx": result = "text/x-la-asf"; break;
        case ".ltx": result = "application/x-latex"; break;
        case ".lzh": result = "application/octet-stream"; break;
        case ".lzx": result = "application/octet-stream"; break;
        case ".m": result = "text/plain"; break;
        case ".m1v": result = "video/mpeg"; break;
        case ".m2a": result = "audio/mpeg"; break;
        case ".m2v": result = "video/mpeg"; break;
        case ".m3u": result = "audio/x-mpequrl"; break;
        case ".man": result = "application/x-troff-man"; break;
        case ".map": result = "application/x-navimap"; break;
        case ".mar": result = "text/plain"; break;
        case ".mbd": result = "application/mbedlet"; break;
        case ".mc$": result = "application/x-magic-cap-package-1.0"; break;
        case ".mcd": result = "application/mcad"; break;
        case ".mcf": result = "text/mcf"; break;
        case ".mcp": result = "application/netmc"; break;
        case ".me": result = "application/x-troff-me"; break;
        case ".mht": result = "message/rfc822"; break;
        case ".mhtml": result = "message/rfc822"; break;
        case ".mid": result = "audio/midi"; break;
        case ".midi": result = "audio/midi"; break;
        case ".mif": result = "application/x-mif"; break;
        case ".mime": result = "message/rfc822"; break;
        case ".mjf": result = "audio/x-vnd.audioexplosion.mjuicemediafile"; break;
        case ".mjpg": result = "video/x-motion-jpeg"; break;
        case ".mm": result = "application/base64"; break;
        case ".mme": result = "application/base64"; break;
        case ".mod": result = "audio/mod"; break;
        case ".moov": result = "video/quicktime"; break;
        case ".mov": result = "video/quicktime"; break;
        case ".movie": result = "video/x-sgi-movie"; break;
        case ".mp2": result = "audio/mpeg"; break;
        case ".mp3": result = "audio/mpeg"; break;
        case ".mpa": result = "audio/mpeg"; break;
        case ".mpc": result = "application/x-project"; break;
        case ".mpe": result = "video/mpeg"; break;
        case ".mpeg": result = "video/mpeg"; break;
        case ".mpg": result = "video/mpeg"; break;
        case ".mpga": result = "audio/mpeg"; break;
        case ".mpp": result = "application/vnd.ms-project"; break;
        case ".mpt": result = "application/vnd.ms-project"; break;
        case ".mpv": result = "application/vnd.ms-project"; break;
        case ".mpx": result = "application/vnd.ms-project"; break;
        case ".mrc": result = "application/marc"; break;
        case ".ms": result = "application/x-troff-ms"; break;
        case ".mv": result = "video/x-sgi-movie"; break;
        case ".my": result = "audio/make"; break;
        case ".mzz": result = "application/x-vnd.audioexplosion.mzz"; break;
        case ".nap": result = "image/naplps"; break;
        case ".naplps": result = "image/naplps"; break;
        case ".nc": result = "application/x-netcdf"; break;
        case ".ncm": result = "application/vnd.nokia.configuration-message"; break;
        case ".nif": result = "image/x-niff"; break;
        case ".niff": result = "image/x-niff"; break;
        case ".nix": result = "application/x-mix-transfer"; break;
        case ".nsc": result = "application/x-conference"; break;
        case ".nvd": result = "application/x-navidoc"; break;
        case ".o": result = "application/octet-stream"; break;
        case ".oda": result = "application/oda"; break;
        case ".omc": result = "application/x-omc"; break;
        case ".omcd": result = "application/x-omcdatamaker"; break;
        case ".omcr": result = "application/x-omcregerator"; break;
        case ".p": result = "text/x-pascal"; break;
        case ".p10": result = "application/pkcs10"; break;
        case ".p12": result = "application/pkcs-12"; break;
        case ".p7a": result = "application/x-pkcs7-signature"; break;
        case ".p7c": result = "application/pkcs7-mime"; break;
        case ".p7m": result = "application/pkcs7-mime"; break;
        case ".p7r": result = "application/x-pkcs7-certreqresp"; break;
        case ".p7s": result = "application/pkcs7-signature"; break;
        case ".part": result = "application/pro_eng"; break;
        case ".pas": result = "text/pascal"; break;
        case ".pbm": result = "image/x-portable-bitmap"; break;
        case ".pcl": result = "application/vnd.hp-pcl"; break;
        case ".pct": result = "image/x-pict"; break;
        case ".pcx": result = "image/x-pcx"; break;
        case ".pdb": result = "chemical/x-pdb"; break;
        case ".pdf": result = "application/pdf"; break;
        case ".pfunk": result = "audio/make"; break;
        case ".pgm": result = "image/x-portable-greymap"; break;
        case ".pic": result = "image/pict"; break;
        case ".pict": result = "image/pict"; break;
        case ".pkg": result = "application/x-newton-compatible-pkg"; break;
        case ".pko": result = "application/vnd.ms-pki.pko"; break;
        case ".pl": result = "text/plain"; break;
        case ".plx": result = "application/x-pixclscript"; break;
        case ".pm": result = "image/x-xpixmap"; break;
        case ".pm4": result = "application/x-pagemaker"; break;
        case ".pm5": result = "application/x-pagemaker"; break;
        case ".png": result = "image/png"; break;
        case ".pnm": result = "application/x-portable-anymap"; break;
        case ".pot": result = "application/vnd.ms-powerpoint"; break;
        case ".pov": result = "model/x-pov"; break;
        case ".ppa": result = "application/vnd.ms-powerpoint"; break;
        case ".ppm": result = "image/x-portable-pixmap"; break;
        case ".pps": result = "application/vnd.ms-powerpoint"; break;
        case ".ppt": result = "application/vnd.ms-powerpoint"; break;
        case ".ppz": result = "application/vnd.ms-powerpoint"; break;
        case ".pre": result = "application/x-freelance"; break;
        case ".prt": result = "application/pro_eng"; break;
        case ".ps": result = "application/postscript"; break;
        case ".psd": result = "application/octet-stream"; break;
        case ".pvu": result = "paleovu/x-pv"; break;
        case ".pwz": result = "application/vnd.ms-powerpoint"; break;
        case ".py": result = "text/x-script.phyton"; break;
        case ".pyc": result = "applicaiton/x-bytecode.python"; break;
        case ".qcp": result = "audio/vnd.qcelp"; break;
        case ".qd3": result = "x-world/x-3dmf"; break;
        case ".qd3d": result = "x-world/x-3dmf"; break;
        case ".qif": result = "image/x-quicktime"; break;
        case ".qt": result = "video/quicktime"; break;
        case ".qtc": result = "video/x-qtc"; break;
        case ".qti": result = "image/x-quicktime"; break;
        case ".qtif": result = "image/x-quicktime"; break;
        case ".ra": result = "audio/x-pn-realaudio"; break;
        case ".ram": result = "audio/x-pn-realaudio"; break;
        case ".ras": result = "application/x-cmu-raster"; break;
        case ".rast": result = "image/cmu-raster"; break;
        case ".rexx": result = "text/x-script.rexx"; break;
        case ".rf": result = "image/vnd.rn-realflash"; break;
        case ".rgb": result = "image/x-rgb"; break;
        case ".rm": result = "application/vnd.rn-realmedia"; break;
        case ".rmi": result = "audio/mid"; break;
        case ".rmm": result = "audio/x-pn-realaudio"; break;
        case ".rmp": result = "audio/x-pn-realaudio"; break;
        case ".rng": result = "application/ringing-tones"; break;
        case ".rnx": result = "application/vnd.rn-realplayer"; break;
        case ".roff": result = "application/x-troff"; break;
        case ".rp": result = "image/vnd.rn-realpix"; break;
        case ".rpm": result = "audio/x-pn-realaudio-plugin"; break;
        case ".rt": result = "text/richtext"; break;
        case ".rtf": result = "text/richtext"; break;
        case ".rtx": result = "text/richtext"; break;
        case ".rv": result = "video/vnd.rn-realvideo"; break;
        case ".s": result = "text/x-asm"; break;
        case ".s3m": result = "audio/s3m"; break;
        case ".saveme": result = "application/octet-stream"; break;
        case ".sbk": result = "application/x-tbook"; break;
        case ".scm": result = "application/x-lotusscreencam"; break;
        case ".sdml": result = "text/plain"; break;
        case ".sdp": result = "application/sdp"; break;
        case ".sdr": result = "application/sounder"; break;
        case ".sea": result = "application/sea"; break;
        case ".set": result = "application/set"; break;
        case ".sgm": result = "text/sgml"; break;
        case ".sgml": result = "text/sgml"; break;
        case ".sh": result = "application/x-sh"; break;
        case ".shar": result = "application/x-shar"; break;
        case ".shtml": result = "text/html"; break;
        case ".sid": result = "audio/x-psid"; break;
        case ".sit": result = "application/x-sit"; break;
        case ".skd": result = "application/x-koan"; break;
        case ".skm": result = "application/x-koan"; break;
        case ".skp": result = "application/x-koan"; break;
        case ".skt": result = "application/x-koan"; break;
        case ".sl": result = "application/x-seelogo"; break;
        case ".smi": result = "application/smil"; break;
        case ".smil": result = "application/smil"; break;
        case ".snd": result = "audio/basic"; break;
        case ".sol": result = "application/solids"; break;
        case ".spc": result = "text/x-speech"; break;
        case ".spl": result = "application/futuresplash"; break;
        case ".spr": result = "application/x-sprite"; break;
        case ".sprite": result = "application/x-sprite"; break;
        case ".src": result = "application/x-wais-source"; break;
        case ".ssi": result = "text/x-server-parsed-html"; break;
        case ".ssm": result = "application/streamingmedia"; break;
        case ".sst": result = "application/vnd.ms-pki.certstore"; break;
        case ".step": result = "application/step"; break;
        case ".stl": result = "application/sla"; break;
        case ".stp": result = "application/step"; break;
        case ".sv4cpio": result = "application/x-sv4cpio"; break;
        case ".sv4crc": result = "application/x-sv4crc"; break;
        case ".svf": result = "image/vnd.dwg"; break;
        case ".svr": result = "application/x-world"; break;
        case ".swf": result = "application/x-shockwave-flash"; break;
        case ".t": result = "application/x-troff"; break;
        case ".talk": result = "text/x-speech"; break;
        case ".tar": result = "application/x-tar"; break;
        case ".tbk": result = "application/toolbook"; break;
        case ".tcl": result = "application/x-tcl"; break;
        case ".tcsh": result = "text/x-script.tcsh"; break;
        case ".tex": result = "application/x-tex"; break;
        case ".texi": result = "application/x-texinfo"; break;
        case ".texinfo": result = "application/x-texinfo"; break;
        case ".text": result = "text/plain"; break;
        case ".tgz": result = "application/x-compressed"; break;
        case ".tif": result = "image/tiff"; break;
        case ".tiff": result = "image/tiff"; break;
        case ".tr": result = "application/x-troff"; break;
        case ".tsi": result = "audio/tsp-audio"; break;
        case ".tsp": result = "application/dsptype"; break;
        case ".tsv": result = "text/tab-separated-values"; break;
        case ".turbot": result = "image/florian"; break;
        case ".txt": result = "text/plain"; break;
        case ".uil": result = "text/x-uil"; break;
        case ".uni": result = "text/uri-list"; break;
        case ".unis": result = "text/uri-list"; break;
        case ".unv": result = "application/i-deas"; break;
        case ".uri": result = "text/uri-list"; break;
        case ".uris": result = "text/uri-list"; break;
        case ".ustar": result = "application/x-ustar"; break;
        case ".uu": result = "application/octet-stream"; break;
        case ".uue": result = "text/x-uuencode"; break;
        case ".vcd": result = "application/x-cdlink"; break;
        case ".vcs": result = "text/x-vcalendar"; break;
        case ".vda": result = "application/vda"; break;
        case ".vdo": result = "video/vdo"; break;
        case ".vew": result = "application/groupwise"; break;
        case ".viv": result = "video/vivo"; break;
        case ".vivo": result = "video/vivo"; break;
        case ".vmd": result = "application/vocaltec-media-desc"; break;
        case ".vmf": result = "application/vocaltec-media-file"; break;
        case ".voc": result = "audio/voc"; break;
        case ".vos": result = "video/vosaic"; break;
        case ".vox": result = "audio/voxware"; break;
        case ".vqe": result = "audio/x-twinvq-plugin"; break;
        case ".vqf": result = "audio/x-twinvq"; break;
        case ".vql": result = "audio/x-twinvq-plugin"; break;
        case ".vrml": result = "application/x-vrml"; break;
        case ".vrt": result = "x-world/x-vrt"; break;
        case ".vsd": result = "application/x-visio"; break;
        case ".vst": result = "application/x-visio"; break;
        case ".vsw": result = "application/x-visio"; break;
        case ".w60": result = "application/wordperfect6.0"; break;
        case ".w61": result = "application/wordperfect6.1"; break;
        case ".w6w": result = "application/msword"; break;
        case ".wav": result = "audio/wav"; break;
        case ".wb1": result = "application/x-qpro"; break;
        case ".wbmp": result = "image/vnd.wap.wbmp"; break;
        case ".web": result = "application/vnd.xara"; break;
        case ".wiz": result = "application/msword"; break;
        case ".wk1": result = "application/x-123"; break;
        case ".wmf": result = "windows/metafile"; break;
        case ".wml": result = "text/vnd.wap.wml"; break;
        case ".wmlc": result = "application/vnd.wap.wmlc"; break;
        case ".wmls": result = "text/vnd.wap.wmlscript"; break;
        case ".wmlsc": result = "application/vnd.wap.wmlscriptc"; break;
        case ".word": result = "application/msword"; break;
        case ".wp": result = "application/wordperfect"; break;
        case ".wp5": result = "application/wordperfect"; break;
        case ".wp6": result = "application/wordperfect"; break;
        case ".wpd": result = "application/wordperfect"; break;
        case ".wq1": result = "application/x-lotus"; break;
        case ".wri": result = "application/mswrite"; break;
        case ".wrl": result = "application/x-world"; break;
        case ".wrz": result = "x-world/x-vrml"; break;
        case ".wsc": result = "text/scriplet"; break;
        case ".wsrc": result = "application/x-wais-source"; break;
        case ".wtk": result = "application/x-wintalk"; break;
        case ".xbm": result = "image/x-xbitmap"; break;
        case ".xdr": result = "video/x-amt-demorun"; break;
        case ".xgz": result = "xgl/drawing"; break;
        case ".xif": result = "image/vnd.xiff"; break;
        case ".xl": result = "application/excel"; break;
        case ".xla": result = "application/vnd.ms-excel"; break;
        case ".xlb": result = "application/vnd.ms-excel"; break;
        case ".xlc": result = "application/vnd.ms-excel"; break;
        case ".xld": result = "application/vnd.ms-excel"; break;
        case ".xlk": result = "application/vnd.ms-excel"; break;
        case ".xll": result = "application/vnd.ms-excel"; break;
        case ".xlm": result = "application/vnd.ms-excel"; break;
        case ".xls": result = "application/vnd.ms-excel"; break;
        case ".xlt": result = "application/vnd.ms-excel"; break;
        case ".xlv": result = "application/vnd.ms-excel"; break;
        case ".xlw": result = "application/vnd.ms-excel"; break;
        case ".xm": result = "audio/xm"; break;
        case ".xml": result = "application/xml"; break;
        case ".xmz": result = "xgl/movie"; break;
        case ".xpix": result = "application/x-vnd.ls-xpix"; break;
        case ".xpm": result = "image/xpm"; break;
        case ".x-png": result = "image/png"; break;
        case ".xsr": result = "video/x-amt-showrun"; break;
        case ".xwd": result = "image/x-xwd"; break;
        case ".xyz": result = "chemical/x-pdb"; break;
        case ".z": result = "application/x-compressed"; break;
        case ".zip": result = "application/zip"; break;
        case ".zoo": result = "application/octet-stream"; break;
        case ".zsh": result = "text/x-script.zsh"; break;
        default: result = "application/octet-stream"; break;
      }
      return result;
    }

    private DateTime? GetDBDate(object o, bool allowNull)
    {
      DateTime? result = allowNull ? null : (DateTime?)DateTime.UtcNow;

      try
      {
        if (o.ToString().Trim() != "") result = DateTime.Parse(o.ToString().Trim());
        result = TimeZoneInfo.ConvertTimeToUtc((DateTime)result);
      }
      catch (Exception)
      {
      }

      return result;
    }

    private int? GetDBInt(object o, bool allowNull)
    {
      int? result = allowNull ? null : (int?)0;

      try
      {
        if (o.ToString().Trim() != "") result = int.Parse(o.ToString().Trim());
      }
      catch (Exception)
      {
      }

      return result;
    }

    private string GetDBString(object o, int maxLength, bool allowNull)
    {
      string result = o.ToString().Trim();
      if (allowNull && result == "") return null;
      if (result.Length > maxLength && maxLength > 0) result = result.Substring(0, maxLength);
      return result;
    }

    private int GetUserContactID(string importID, Users users)
    {
      string id = importID;
      if (importID.ToLower().IndexOf("[contact]") > -1)
      {
        importID = importID.Substring(9, importID.Length - 9);

      }

      User user = users.FindByImportID(importID);
      if (user != null)
        return user.UserID;
      else
        return -1;
    }

    private bool GetSettingBool(string key)
    {
      return GetSettingString(key).ToLower() == "true";
    }

    private string GetSettingString(string key)
    {
      DataRow row = ReadRow("Settings", "Key", key);
      if (row == null) throw new Exception("Missing Setting: " + key);
      return row[1].ToString().Trim();
    }

    private int GetSettingInt(string key)
    {
      return int.Parse(GetSettingString(key));
    }

    private IdList GetIdList(BaseCollection collection)
    {
      IdList result = new SortedList<string, int>();
      foreach (DataRow row in collection.Table.Rows)
      {
        string importedID = row["ImportID"] != DBNull.Value ? row["ImportID"].ToString().Trim().ToLower() : "";
        if (importedID != "")
        {
          try
          {
            result.Add(importedID, (int)row[collection.PrimaryKeyFieldName]);
          }
          catch
          {
          }
        }
      }

      return result;
    }

    #endregion

    #region Import Methods
    private void ImportUsers()
    {
      Users users = new Users(_loginUser);
      Users existing = new Users(_loginUser);
      existing.LoadByOrganizationID(_organizationID, false);
      DataTable table = ReadTable("Users");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        if (existing.FindByImportID(row["UserID"].ToString().Trim()) != null) continue;
        User user = users.AddNewUser();
        user.ActivatedOn = DateTime.UtcNow;
        user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(row["Password"].ToString().Trim(), "MD5");
        user.DeactivatedOn = null;
        user.Email = row["Email"].ToString().Trim();
        user.FirstName = row["FirstName"].ToString().Trim();
        user.ImportID = row["UserID"].ToString().Trim();
        user.InOffice = false;
        user.InOfficeComment = "";
        user.IsActive = bool.Parse(row["IsActive"].ToString().Trim());
        user.IsFinanceAdmin = bool.Parse(row["IsFinanceAdmin"].ToString().Trim());
        user.IsSystemAdmin = bool.Parse(row["IsSystemAdmin"].ToString().Trim());
        user.IsPasswordExpired = true;
        user.IsPortalUser = true;
        user.LastActivity = DateTime.UtcNow;
        user.LastLogin = DateTime.UtcNow;
        user.LastName = row["LastName"].ToString().Trim();
        user.MiddleName = row["MiddleName"].ToString().Trim();
        user.OrganizationID = _organizationID;
        user.PrimaryGroupID = null;
        user.SubscribeToNewTickets = false;
        user.ReceiveTicketNotifications = true;
        user.Title = row["Title"].ToString().Trim();
      }
      users.BulkSave();
      _log.AppendMessage(users.Count.ToString() + " Users Imported.");

    }

    private void ImportGroups()
    {
      Groups groups = new Groups(_loginUser);
      Groups existing = new Groups(_loginUser);
      existing.LoadByOrganizationID(_organizationID);
      DataTable table = ReadTable("Groups");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Group group = existing.FindByImportID(row["GroupID"].ToString().Trim());
        if (group != null) continue;
        group = existing.FindByName(row["Name"].ToString());
        if (group == null)
        {
          group = groups.AddNewGroup();
          group.Description = row["Description"].ToString().Trim();
          group.Name = row["Name"].ToString().Trim();
          group.OrganizationID = _organizationID;
          group.ImportID = row["GroupID"].ToString().Trim();
        }
        else
        {
          group.ImportID = row["GroupID"].ToString().Trim();
          group.Collection.Save();

        }

      }
      groups.BulkSave();
      _log.AppendMessage(groups.Count.ToString() + " Groups Imported.");

    }

    private void ImportUserGroups()
    {
      Users users = new Users(_loginUser);
      users.LoadByOrganizationID(_loginUser.OrganizationID, false);
      Groups groups = new Groups(_loginUser);
      groups.LoadByOrganizationID(_loginUser.OrganizationID);

      int count = 0;
      DataTable table = ReadTable("UserGroups");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Group group = groups.FindByImportID(row["GroupID"].ToString().Trim());
        User user = users.FindByImportID(row["UserID"].ToString().Trim());
        if (user != null && group != null)
        {
          count++;
          users.AddUserGroup(user.UserID, group.GroupID);
        }
        else
        {
          _log.AppendError(row, "User Group skipped due to invalid group or user id.");
        }
      }
      _log.AppendMessage(count.ToString() + " User Groups Imported.");

    }

    private void ImportCustomers()
    {
      Organizations organizations = new Organizations(_loginUser);
      Organizations existing = new Organizations(_loginUser);
      existing.LoadByParentID(_organizationID, false);
      IdList list = GetIdList(existing);
      DataTable table = ReadTable("Customers");
      int count = 0;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        if (list.ContainsKey(row["CustomerID"].ToString().Trim())) continue;
        //if (existing.FindByImportID(row["CustomerID"].ToString().Trim()) != null) continue;
        Organization organization = organizations.AddNewOrganization();
        organization.Description = "";
        organization.ExtraStorageUnits = 0;
        organization.HasPortalAccess = false;
        organization.ImportID = row["CustomerID"].ToString().Trim();
        organization.InActiveReason = "";
        organization.IsActive = true;
        organization.IsCustomerFree = false;
        organization.Name = row["Name"].ToString().Trim();
        organization.ParentID = _organizationID;
        organization.PortalSeats = 0;
        organization.PrimaryUserID = null;
        organization.ProductType = ProductType.Express;
        organization.SystemEmailID = Guid.NewGuid();
        organization.UserSeats = 0;
        organization.CompanyDomains = row["Domains"].ToString().Trim();
        organization.WebServiceID = Guid.NewGuid();
        organization.Website = row["Website"].ToString().Trim();
        organization.SAExpirationDate = GetDBDate(row["ServiceExpiration"], true);
        if (++count % BULK_LIMIT == 0)
        {
          organizations.BulkSave();
          organizations = new Organizations(_loginUser);
          GC.WaitForPendingFinalizers();
        }
      }

      organizations.BulkSave();
      _log.AppendMessage(count.ToString() + " Customers Imported.");
    }

    private void ImportContacts()
    {
      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);
      IdList idList = GetIdList(organizations);

      Users users = new Users(_loginUser);
      DataTable table = ReadTable("Contacts");
      int count = 0;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        int organizationID;

        if (!idList.TryGetValue(row["CustomerID"].ToString().Trim().ToLower(), out organizationID))
        {
          _log.AppendError(row, "Contact skipped due to missing organization.");
          continue;

        }
        //Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());
        //if (organization == null) { _log.AppendError(row, "Contact skipped due to missing organization."); continue; }

        User user = users.AddNewUser();
        user.ActivatedOn = DateTime.UtcNow;
        user.CryptedPassword = "";
        user.DeactivatedOn = null;
        user.Email = row["Email"].ToString().Trim();
        user.FirstName = row["FirstName"].ToString().Trim();
        user.ImportID = "[contact]" + row["ContactID"].ToString().Trim();
        user.InOffice = false;
        user.InOfficeComment = "";
        user.IsActive = true;
        user.IsFinanceAdmin = false;
        user.IsPasswordExpired = true;
        user.IsPortalUser = false;
        user.IsSystemAdmin = false;
        user.LastActivity = DateTime.UtcNow;
        user.LastLogin = DateTime.UtcNow;
        user.LastName = row["LastName"].ToString().Trim();
        user.MiddleName = row["MiddleName"].ToString().Trim();
        user.OrganizationID = organizationID;
        user.PrimaryGroupID = null;
        user.Title = row["Title"].ToString().Trim();
        if (++count % BULK_LIMIT == 0)
        {
          users.BulkSave();
          users = new Users(_loginUser);
          GC.WaitForPendingFinalizers();
        }

      }
      _log.AppendMessage(count.ToString() + " Contacts Imported.");
      users.BulkSave();
    }

    private void ImportPrimaryContacts()
    {
      Users users = new Users(_loginUser);
      users.LoadContacts(_loginUser.OrganizationID, false);
      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_loginUser.OrganizationID, false);

      int count = 0;
      DataTable table = ReadTable("PrimaryContacts");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());
        User user = users.FindByImportID("[contact]" + row["ContactID"].ToString().Trim());
        if (user != null && organization != null)
        {
          count++;
          organization.PrimaryUserID = user.UserID;
        }
        else
        {
          _log.AppendError(row, "Primary contact skipped due to invalid customer or contact id.");
        }
      }
      organizations.Save();
      _log.AppendMessage(count.ToString() + " Primary Contacts Imported.");
    }

    private void ImportProducts()
    {
      Products products = new Products(_loginUser);
      Products existing = new Products(_loginUser);
      existing.LoadByOrganizationID(_organizationID);

      DataTable table = ReadTable("Products");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Product p = existing.FindByImportID(row["ProductID"].ToString().Trim());
        if (p != null)
        {
          p.ImportID = row["ProductID"].ToString().Trim();
          p.Collection.Save();
        }
        else
        {
          Product product = products.AddNewProduct();
          product.Description = row["Description"].ToString().Trim();
          product.ImportID = row["ProductID"].ToString().Trim();
          product.Name = row["Name"].ToString().Trim();
          product.OrganizationID = _organizationID;
        }
      }

      products.BulkSave();
      _log.AppendMessage(products.Count.ToString() + " Products Imported.");
    }

    private void ImportProductVersions()
    {
      Products products = new Products(_loginUser);
      products.LoadByOrganizationID(_organizationID);

      ProductVersionStatuses statuses = new ProductVersionStatuses(_loginUser);
      statuses.LoadAllPositions(_organizationID);

      ProductVersions productVersions = new ProductVersions(_loginUser);
      ProductVersions existing = new ProductVersions(_loginUser);
      existing.LoadByParentOrganizationID(_organizationID);

      DataTable table = ReadTable("Versions");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Product product = products.FindByImportID(row["ProductID"].ToString().Trim());
        if (product == null)
        {
          _log.AppendError(row, "Version skipped due to missing product.");
          continue;
        }
        if (existing.FindByImportID(row["VersionID"].ToString().Trim()) != null) continue;

        ProductVersion productVersion = productVersions.AddNewProductVersion();
        productVersion.Description = row["Description"].ToString().Trim();
        productVersion.ImportID = row["VersionID"].ToString().Trim();
        productVersion.IsReleased = bool.Parse(row["IsReleased"].ToString().Trim());
        productVersion.ProductID = product.ProductID;
        productVersion.ProductVersionStatusID = GetVersionStatus(statuses, row["ProductVersionStatus"].ToString()).ProductVersionStatusID;
        productVersion.ReleaseDate = GetDBDate(row["ReleaseDate"], true);
        productVersion.VersionNumber = row["VersionNumber"].ToString().Trim();
      }

      productVersions.BulkSave();
      _log.AppendMessage(productVersions.Count.ToString() + " Versions Imported.");
    }


    private void ImportAssets()
    {
      Products products = new Products(_loginUser);
      products.LoadByOrganizationID(_organizationID);
      IdList prodIds = GetIdList(products);

      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);
      IdList orgIDs = GetIdList(organizations);

      Assets assets = new Assets(_loginUser);
      int orgCount = 0;
      int prodCount = 0;

      DataTable table = ReadTable("Assets");
      int count = 0;
      foreach (DataRow row in table.Rows)
      {

        _currentRow = row;

        Asset asset = assets.AddNewAsset();
        asset.OrganizationID = _organizationID;
        asset.SerialNumber = GetDBString(row["SerialNumber"], 500, true);
        asset.Name = GetDBString(row["Name"], 500, true);
        switch (GetDBString(row["Location"], 0, false).ToLower().Trim())
	      {
          case "assigned": asset.Location = "1"; break;
          case "warehouse": asset.Location = "2"; break;
          case "junkyard": asset.Location = "3"; break;
		      default: asset.Location = ""; break;
	      }
        asset.Notes = GetDBString(row["Notes"], 0, true);

        string productID = row["ProductID"].ToString().Trim().ToLower();
        if (productID != "")
        {
          Product product = products.FindByImportID(productID);
          if (product == null)
          {
            product = (new Products(_loginUser)).AddNewProduct();
            product.Name = row["ProductID"].ToString();
            product.OrganizationID = _organizationID;
            product.ImportID = product.Name;
            product.Collection.Save();
            prodCount++;
            products.LoadByOrganizationID(_organizationID);
          }
          asset.ProductID = product.ProductID;
        }
        else
        {
          asset.ProductID = null;
        }


        string organizationID = row["AssignedTo"].ToString().Trim().ToLower();
        if (organizationID != "")
        {
          Organization organization = organizations.FindByImportID(organizationID);
          if (organization == null)
          {
            organization = (new Organizations(_loginUser)).AddNewOrganization();
            organization.Name = row["AssignedTo"].ToString();
            organization.ParentID = _organizationID;
            organization.ImportID = organization.Name;
            organization.IsActive = true;
            organization.HasPortalAccess = false;
            organization.Collection.Save();
            orgCount++;
            organizations.LoadByParentID(_organizationID, false);
          }
          asset.AssignedTo = organization.OrganizationID;
        }
        else
        {
          asset.AssignedTo = null;
        }
        

        asset.WarrantyExpiration = GetDBDate(row["WarrantyExpiration"], true);
        asset.CreatorID = -1;
        asset.ModifierID = -1;
        asset.DateModified = DateTime.UtcNow;
        asset.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
        asset.SubPartOf = null;
        asset.Status = GetDBString(row["Status"], 500, true);
        asset.ImportID = row["AssetID"].ToString().Trim();

        if (++count % BULK_LIMIT == 0)
        {
          assets.BulkSave();
          assets = new Assets(_loginUser);
          GC.WaitForPendingFinalizers();

        }
      }
      assets.BulkSave();

      _log.AppendMessage(count.ToString() + " Assets Imported.");

      _log.AppendMessage(orgCount.ToString() + " Assets Customers Imported.");
      _log.AppendMessage(prodCount.ToString() + " Assets Products Imported.");

    }

    private void ImportTickets()
    {
      TicketTypes ticketTypes = new TicketTypes(_loginUser);
      ticketTypes.LoadAllPositions(_organizationID);
      foreach (TicketType ticketType in ticketTypes)
      {
        if (_ticketTypeNames.Contains(ticketType.Name)) ImportTicketsByTicketType(ticketType);
      }
    }

    private void ImportTicketsByTicketType(TicketType ticketType)
    {
      Users users = new Users(_loginUser);
      users.LoadContactsAndUsers(_organizationID, false);

      Groups groups = new Groups(_loginUser);
      groups.LoadByOrganizationID(_organizationID);

      Products products = new Products(_loginUser);
      products.LoadByOrganizationID(_organizationID);

      ProductVersions productVersions = new ProductVersions(_loginUser);
      productVersions.LoadByParentOrganizationID(_organizationID);

      TicketStatuses ticketStatuses = new TicketStatuses(_loginUser);
      ticketStatuses.LoadAllPositions(ticketType.TicketTypeID);

      TicketSeverities ticketSeverities = new TicketSeverities(_loginUser);
      ticketSeverities.LoadAllPositions(_organizationID);

      Tickets tickets = new Tickets(_loginUser);

      int maxTicketNumber = tickets.GetMaxTicketNumber(_organizationID);
      if (maxTicketNumber < 0) maxTicketNumber++;
      int count = 0;
      DataTable table = ReadTable(ticketType.Name);
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        int ticketNumber;

        if (row.Table.Columns.Contains("TicketNumber") && row["TicketNumber"].ToString().Trim() != "")
        {
          ticketNumber = int.Parse(row["TicketNumber"].ToString());
        }
        else
        {
          ticketNumber = maxTicketNumber + 1;
        }

        maxTicketNumber = Math.Max(ticketNumber, maxTicketNumber);

        User assignedTo = users.Find(row["UserID"].ToString().Trim());
        int? userID = assignedTo == null ? null : (int?)assignedTo.UserID;

        Group group = groups.FindByImportID(row["GroupID"].ToString().Trim());
        int? groupID = group == null ? null : (int?)group.GroupID;

        User creator = users.FindByImportID(row["CreatorID"].ToString());
        int creatorID = creator == null ? -1 : creator.UserID;

        User modifier = users.FindByImportID(row["ModifierID"].ToString());
        int modifierID = modifier == null ? -1 : modifier.UserID;

        User closer = users.FindByImportID(row["CloserID"].ToString());
        int? closerID = closer == null ? -1 : closer.UserID;

        Product product = products.FindByImportID(row["ProductID"].ToString().Trim());
        int? productID = product == null ? null : (int?)product.ProductID;

        ProductVersion reported = productVersions.FindByImportID(row["ReportedVersionID"].ToString().Trim());
        int? reportedID = reported == null ? null : (int?)reported.ProductVersionID;

        ProductVersion resolved = productVersions.FindByImportID(row["ResolvedVersionID"].ToString().Trim());
        int? resolvedID = resolved == null ? null : (int?)resolved.ProductVersionID;

        Ticket ticket = tickets.AddNewTicket();
        ticket.CloserID = closerID;
        ticket.CreatorID = creatorID;
        ticket.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
        ticket.ModifierID = modifierID;
        ticket.DateModified = (DateTime)GetDBDate(row["DateModified"], false);
        ticket.DateClosed = GetDBDate(row["DateClosed"], true);
        ticket.GroupID = null;
        ticket.ImportID = row["TicketID"].ToString().Trim();
        ticket.IsKnowledgeBase = false;
        ticket.IsVisibleOnPortal = false;
        ticket.ModifierID = _loginUser.UserID;
        ticket.Name = GetDBString(row["Name"], 250, false);
        ticket.OrganizationID = _organizationID;
        ticket.ParentID = null;
        ticket.ProductID = productID;
        ticket.ReportedVersionID = reportedID;
        ticket.SolvedVersionID = resolvedID;
        ticket.TicketNumber = ticketNumber;
        ticket.TicketSeverityID = GetTicketSeverity(ticketSeverities, row["TicketSeverity"].ToString()).TicketSeverityID;
        ticket.TicketStatusID = GetTicketStatus(ticketStatuses, row["TicketStatus"].ToString(), ticketType).TicketStatusID;
        ticket.TicketTypeID = ticketType.TicketTypeID;
        ticket.UserID = userID;
        ticket.GroupID = groupID;

        if (++count % BULK_LIMIT == 0)
        {
          tickets.BulkSave();
          tickets = new Tickets(_loginUser);
          GC.WaitForPendingFinalizers();

        }
      }
      tickets.BulkSave();
      EmailPosts.DeleteImportEmails(_loginUser);

      _log.AppendMessage(count.ToString() + " " + ticketType.Name + " Imported.");

    }

    private void ImportActions()
    {
      Users users = new Users(_loginUser);
      //users.LoadByOrganizationID(_organizationID, false);
      users.LoadContactsAndUsers(_organizationID, false);

      Tickets tickets = new Tickets(_loginUser);
      tickets.LoadByOrganizationID(_organizationID);

      ActionTypes actionTypes = new ActionTypes(_loginUser);
      actionTypes.LoadAllPositions(_organizationID);


      Actions actions = new Actions(_loginUser);
      int count = 0;
      DataTable table = ReadTable("Actions");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Ticket ticket = tickets.FindByImportID(row["TicketID"].ToString().Trim());
        if (ticket == null)
        {
          _log.AppendError(row, "Action skipped due to missing ticket");
          continue;
        }

        User creator = users.FindByImportID(row["CreatorID"].ToString());
        int creatorID = creator == null ? -1 : creator.UserID;
        //int creatorID = GetUserContactID(row["CreatorID"].ToString(), users);

        TeamSupport.Data.Action action = actions.AddNewAction();

        action.SystemActionTypeID = GetSystemActionTypeID(row["ActionType"].ToString());
        action.ActionTypeID = GetActionTypeID(actionTypes, row["ActionType"].ToString());
        action.CreatorID = creatorID;
        action.DateCreated = (DateTime)GetDBDate(row["DateCreated"].ToString().Trim(), false);
        action.DateModified = DateTime.UtcNow;
        action.DateStarted = GetDBDate(row["DateStarted"].ToString().Trim(), true);
        string desc = row["Description"].ToString().Trim();
        action.Description = desc == "" ? "Comment" : desc;
        action.IsVisibleOnPortal = row["VisibleOnPortal"].ToString().ToLower().IndexOf("t") > -1;
        action.ModifierID = _loginUser.UserID;
        action.Name = GetDBString(row["Name"], 500, false);
        if (action.Name.Length > 499) action.Name = action.Name.Substring(0, 499);
        action.TicketID = ticket.TicketID;
        action.ImportID = row["ActionID"].ToString().Trim();
        action.TimeSpent = GetDBInt(row["TimeSpent"], true);
        if (action.IsVisibleOnPortal && !ticket.IsVisibleOnPortal) ticket.IsVisibleOnPortal = true;

        if (++count % BULK_LIMIT == 0)
        {
          tickets.Save();
          EmailPosts.DeleteImportEmails(_loginUser);
          actions.BulkSave();
          actions = new Actions(_loginUser);
          GC.WaitForPendingFinalizers();

        }

      }
      tickets.Save();
      EmailPosts.DeleteImportEmails(_loginUser);
      actions.BulkSave();
      _log.AppendMessage(count.ToString() + " Actions Imported.");
    }

    private void ImportAttachments()
    {
      Tickets tickets = new Tickets(_loginUser);
      tickets.LoadByOrganizationID(_organizationID);
      Actions actions = new Actions(_loginUser);
      actions.LoadByOrganizationID(_organizationID);

      Attachments attachments = new Attachments(_loginUser);
      DataTable table = ReadTable("Attachments");
      foreach (DataRow row in table.Rows)
      {
        ReferenceType refType = ReferenceType.None;
        int id = -1;
        _currentRow = row;
        string sourceFile = Path.Combine(Path.Combine(Path.GetDirectoryName(_fileName), row["Folder"].ToString().Trim()), row["FileName"].ToString().Trim());
        if (!File.Exists(sourceFile))
        {
          _log.AppendError(row, "Attachment skipped due to file does not exist.");
          continue;
        }


        string objectID = row["ObjectID"].ToString().Trim();
        if (row["ReferenceObject"].ToString().ToLower().IndexOf("ticket") > -1)
        {
          Ticket ticket = tickets.FindByImportID(objectID);
          if (ticket == null)
          {
            _log.AppendError(row, "Attachment skipped due to ticket does not exist.");
            continue;
          }

          TeamSupport.Data.Action action = Actions.GetTicketDescription(_loginUser, ticket.TicketID);
          if (action == null)
          {
            _log.AppendError(row, "Attachment skipped due to action description does not exist.");
            continue;
          }
          refType = ReferenceType.Actions;
          id = action.ActionID;
        }
        else if (row["ReferenceObject"].ToString().ToLower().IndexOf("action") > -1)
        {
          TeamSupport.Data.Action action = actions.FindByImportID(objectID);
          if (action == null)
          {
            _log.AppendError(row, "Attachment skipped due to action does not exist.");
            continue;
          }
          refType = ReferenceType.Actions;
          id = action.ActionID;
        }
        else
        {
          _log.AppendError(row, "Attachment skipped due to invalid reference object.");
          continue;

        }

        string path = Attachments.GetAttachmentPath(_loginUser, refType, id);
        Directory.CreateDirectory(path);
        string ext = Path.GetExtension(sourceFile);
        string fileName = Path.GetFileName(sourceFile);


        try
        {
          FileInfo info = new FileInfo(sourceFile);
          info.CopyTo(Path.Combine(path, fileName));
          Attachment attachment = attachments.AddNewAttachment();
          attachment.FileName = fileName;
          attachment.FileSize = info.Length;
          attachment.FileType = MimeType(fileName);
          attachment.OrganizationID = _organizationID;
          attachment.Path = Path.Combine(path, fileName);
          attachment.RefID = id;
          attachment.RefType = refType;
        }
        catch (Exception ex)
        {
          _log.AppendError(row, "Attachment skipped due to error: " + ex.Message);

        }
      }
      attachments.BulkSave();
      _log.AppendMessage(attachments.Count.ToString() + " Attachments Imported.");

    }

    private void ImportWiki()
    {

      Users users = new Users(_loginUser);
      users.LoadContactsAndUsers(_organizationID, false);

      WikiArticles wikis = new WikiArticles(_loginUser);
      Dictionary<string, int> list = new Dictionary<string, int>();
      Dictionary<int, string> parents = new Dictionary<int, string>();

      int count = 0;
      DataTable table = ReadTable("Wiki");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;

        User creator = users.FindByImportID(row["CreatedBy"].ToString().Trim());
        int? creatorID = creator == null ? null : (int?)creator.UserID;

        User modifier = users.FindByImportID(row["ModifiedBy"].ToString().Trim());
        int? modifierID = modifier == null ? null : (int?)modifier.UserID;

        WikiArticle wiki = wikis.AddNewWikiArticle();
        wiki.ArticleName = GetDBString(row["ArticleName"], 500, true);
        wiki.Body = row["Body"].ToString();
        wiki.CreatedBy = creatorID;
        wiki.CreatedDate = GetDBDate(row["CreatedDate"], true);
        wiki.ModifiedBy = modifierID;
        wiki.ModifiedDate = GetDBDate(row["ModifiedDate"], true);
        wiki.OrganizationID = _organizationID;
        wiki.ParentID = null;
        wiki.PortalEdit = row["PortalEdit"].ToString().ToLower().IndexOf("t") > -1;
        wiki.PortalView = row["PortalView"].ToString().ToLower().IndexOf("t") > -1;
        wiki.Private = row["Private"].ToString().ToLower().IndexOf("t") > -1;
        wiki.PublicEdit = row["PublicEdit"].ToString().ToLower().IndexOf("t") > -1;
        wiki.PublicView = row["PublicView"].ToString().ToLower().IndexOf("t") > -1;
        wiki.IsDeleted = row["IsDeleted"].ToString().ToLower().IndexOf("t") > -1;
        wiki.Version = GetDBInt(row["Version"], true) == null ? 1 : GetDBInt(row["Version"], true);
        wiki.Collection.Save();
        list.Add(row["WikiID"].ToString(), wiki.ArticleID);
        parents.Add(wiki.ArticleID, row["ParentID"].ToString());

        count++;
      }

      foreach (WikiArticle wiki in wikis)
      {
        try
        {
          wiki.ParentID = list[parents[wiki.ArticleID]];
        }
        catch
        {
        }
      }
      wikis.Save();

      _log.AppendMessage(count.ToString() + " Wiki Articles Imported.");


    }

    private void ImportAddresses()
    {
      Users users = new Users(_loginUser);
      users.LoadContactsAndUsers(_organizationID, false);

      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);

      IdList userList = GetIdList(users);
      IdList orgList = GetIdList(organizations);


      Addresses addresses = new Addresses(_loginUser);
      DataTable table = ReadTable("Addresses");
      int count = 0;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        ReferenceType refType;
        string objectID = row["ObjectID"].ToString().Trim().ToLower();
        int refID;

        if (row["ReferenceObject"].ToString().ToLower().IndexOf("user") > -1)
        {
          refType = ReferenceType.Users;
          int userID;
          if (!userList.TryGetValue(objectID, out userID))
          {
            _log.AppendError(row, "Address skipped due to missing user.");
            continue;
          }
          refID = userID;
        }
        else if (row["ReferenceObject"].ToString().ToLower().IndexOf("contact") > -1)
        {
          refType = ReferenceType.Users;
          int userID;
          if (!userList.TryGetValue("[contact]" + objectID, out userID))
          {
            _log.AppendError(row, "Address skipped due to missing contact.");
            continue;
          }
          refID = userID;
        }
        else
        {
          refType = ReferenceType.Organizations;
          int orgID;
          if (!orgList.TryGetValue(objectID, out orgID))
          {
            _log.AppendError(row, "Address skipped due to missing organization.");
            continue;
          }

          refID = orgID;
        }

        Address address = addresses.AddNewAddress();
        address.Addr1 = row["AddressLine1"].ToString().Trim();
        address.Addr2 = row["AddressLine2"].ToString().Trim();
        address.Addr3 = row["AddressLine3"].ToString().Trim();
        address.City = row["City"].ToString().Trim();
        address.Comment = "";
        address.Country = row["Country"].ToString().Trim();
        address.Description = row["Description"].ToString().Trim();
        address.RefID = refID;
        address.RefType = refType;
        address.State = row["State"].ToString().Trim();
        address.Zip = row["Zip"].ToString().Trim();

        if (++count % BULK_LIMIT == 0)
        {
          addresses.BulkSave();
          addresses = new Addresses(_loginUser);
          GC.WaitForPendingFinalizers();
        }

      }

      addresses.BulkSave();
      _log.AppendMessage(count.ToString() + " Addresses Imported.");

    }

    private void ImportPhoneNumbers()
    {
      Users users = new Users(_loginUser);
      users.LoadContactsAndUsers(_organizationID, false);

      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);

      PhoneTypes phoneTypes = new PhoneTypes(_loginUser);
      phoneTypes.LoadAllPositions(_organizationID);

      IdList userList = GetIdList(users);
      IdList orgList = GetIdList(organizations);

      PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
      DataTable table = ReadTable("PhoneNumbers");
      int count = 0;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        ReferenceType refType;
        string objectID = row["ObjectID"].ToString().Trim().ToLower();
        int refID;

        if (row["ReferenceObject"].ToString().ToLower().IndexOf("user") > -1)
        {
          refType = ReferenceType.Users;
          int userID;
          if (!userList.TryGetValue(objectID, out userID))
          {
            _log.AppendError(row, "Phone number skipped due to missing user.");
            continue;
          }
          refID = userID;
        }
        else if (row["ReferenceObject"].ToString().ToLower().IndexOf("contact") > -1)
        {
          refType = ReferenceType.Users;
          int userID;
          if (!userList.TryGetValue("[contact]" + objectID, out userID))
          {
            _log.AppendError(row, "Phone number skipped due to missing contact.");
            continue;
          }
          refID = userID;
        }
        else
        {
          refType = ReferenceType.Organizations;
          //Organization organization = organizations.FindByImportID(objectID);
          int orgID;
          if (!orgList.TryGetValue(objectID, out orgID))
          {
            _log.AppendError(row, "Phone number skipped due to missing organization.");
            continue;
          }

          refID = orgID;
        }

        PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
        phoneNumber.Extension = row["Extension"].ToString().Trim();
        phoneNumber.OtherTypeName = "";
        phoneNumber.Number = row["PhoneNumber"].ToString().Trim();
        phoneNumber.PhoneTypeID = GetPhoneType(phoneTypes, row["PhoneType"].ToString()).PhoneTypeID;
        phoneNumber.RefID = refID;
        phoneNumber.RefType = refType;

        if (++count % BULK_LIMIT == 0)
        {
          phoneNumbers.BulkSave();
          phoneNumbers = new PhoneNumbers(_loginUser);
          GC.WaitForPendingFinalizers();
        }

      }

      phoneNumbers.BulkSave();
      _log.AppendMessage(count.ToString() + " Phone Numbers Imported.");
    }

    private void ImportNotes()
    {
      Users users = new Users(_loginUser);
      users.LoadByOrganizationID(_organizationID, false);

      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);


      Notes notes = new Notes(_loginUser);

      DataTable table = ReadTable("CustomerNotes");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());
        if (organization == null)
        {
          _log.AppendError(row, "Note skipped due to missing organization.");
          continue;
        }

        User creator = users.FindByImportID(row["CreatorID"].ToString().Trim());
        int creatorID = creator == null ? _loginUser.UserID : creator.UserID;

        Note note = notes.AddNewNote();
        note.CreatorID = creatorID;
        note.DateCreated = (DateTime)GetDBDate(row["DateCreated"], false);
        note.Description = GetDBString(row["Description"], 0, false);
        note.RefID = organization.OrganizationID;
        note.RefType = ReferenceType.Organizations;
        note.Title = row["Title"].ToString().Trim();
      }

      notes.BulkSave();
      _log.AppendMessage(notes.Count.ToString() + " Notes Imported.");

    }

    private void ImportCustomerProducts()
    {
      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);

      Products products = new Products(_loginUser);
      products.LoadByOrganizationID(_organizationID);

      ProductVersions productVersions = new ProductVersions(_loginUser);
      productVersions.LoadByParentOrganizationID(_organizationID);

      OrganizationProducts organizationProducts = new OrganizationProducts(_loginUser);
      DataTable table = ReadTable("CustomerProducts");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Product product = products.FindByImportID(row["ProductID"].ToString().Trim());
        Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());
        ProductVersion version = productVersions.FindByImportID(row["VersionID"].ToString().Trim());

        if (organization == null || product == null)
        {
          _log.AppendError(row, "Customer Product skipped due to missing organization or product.");
          continue;
        }

        OrganizationProduct organizationProduct = organizationProducts.AddNewOrganizationProduct();
        organizationProduct.ImportID = row["UniqueID"].ToString().Trim();
        organizationProduct.IsVisibleOnPortal = false;
        organizationProduct.OrganizationID = organization.OrganizationID;
        organizationProduct.ProductID = product.ProductID;
        organizationProduct.ProductVersionID = version == null ? null : (int?)version.ProductVersionID;
      }
      organizationProducts.BulkSave();

      _log.AppendMessage(organizationProducts.Count.ToString() + " Customer Products Imported.");

    }

    private void ImportCustomerTickets()
    {
      Organizations organizations = new Organizations(_loginUser);
      organizations.LoadByParentID(_organizationID, false);

      Tickets tickets = new Tickets(_loginUser);
      tickets.LoadByOrganizationID(_organizationID);
      DataTable table = ReadTable("CustomerTickets");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Ticket ticket = tickets.FindByImportID(row["TicketID"].ToString().Trim());
        Organization organization = organizations.FindByImportID(row["CustomerID"].ToString().Trim());

        if (organization == null || ticket == null)
        {
          _log.AppendError(row, "Customer Ticket skipped due to missing organization or ticket.");
          continue;
        }

        tickets.AddOrganization(organization.OrganizationID, ticket.TicketID);

      }
    }

    private void ImportContactTickets()
    {
      Users users = new Users(_loginUser);
      //users.LoadByOrganizationID(_organizationID, false);
      users.LoadContactsAndUsers(_organizationID, false);

      Tickets tickets = new Tickets(_loginUser);
      tickets.LoadByOrganizationID(_organizationID);
      DataTable table = ReadTable("ContactTickets");
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        Ticket ticket = tickets.FindByImportID(row["TicketID"].ToString().Trim());
        User user = users.FindByImportID("[contact]" + row["ContactID"].ToString().Trim());

        if (user == null || ticket == null)
        {
          _log.AppendError(row, "Contact Ticket skipped due to missing user or ticket.");
          continue;
        }

        tickets.AddContact(user.UserID, ticket.TicketID);


      }
    }

    private void ImportCustomFields(ReferenceType refType, List<ImportCustomField> importFields, int auxID)
    {
      CustomValues values = new CustomValues(_loginUser);
      IdList idList = null;
      string fieldName = "";
      string tableName = "";
      string idPrefix = "";

      switch (refType)
      {
        case ReferenceType.OrganizationProducts:
          OrganizationProducts organizationProducts = new OrganizationProducts(_loginUser);
          organizationProducts.LoadByParentOrganizationID(_organizationID);
          idList = GetIdList(organizationProducts);
          fieldName = "UniqueID";
          break;
        case ReferenceType.Organizations:
          Organizations organizations = new Organizations(_loginUser);
          organizations.LoadByParentID(_organizationID, false);
          idList = GetIdList(organizations);
          fieldName = "CustomerID";
          break;
        case ReferenceType.Products:
          Products products = new Products(_loginUser);
          products.LoadByOrganizationID(_organizationID);
          idList = GetIdList(products);
          fieldName = "ProductID";
          break;
        case ReferenceType.ProductVersions:
          ProductVersions productVersions = new ProductVersions(_loginUser);
          productVersions.LoadByParentOrganizationID(_organizationID);
          idList = GetIdList(productVersions);
          fieldName = "VersionID";
          break;
        case ReferenceType.Tickets:
          Tickets tickets = new Tickets(_loginUser);
          tickets.LoadByOrganizationID(_organizationID);
          idList = GetIdList(tickets);
          fieldName = "TicketID";
          break;
        case ReferenceType.Users:
          Users users = new Users(_loginUser);
          users.LoadContactsAndUsers(_organizationID, false);
          idList = GetIdList(users);
          fieldName = "UserID";
          break;
        case ReferenceType.Contacts:
          Users contacts = new Users(_loginUser);
          contacts.LoadContactsAndUsers(_organizationID, false);
          idList = GetIdList(contacts);
          fieldName = "ContactID";
          idPrefix = "[contact]";
          break;
        case ReferenceType.Assets:
          Assets assets = new Assets(_loginUser);
          assets.LoadByOrganizationID(_organizationID);
          idList = GetIdList(assets);
          fieldName = "AssetID";
          break;
        default:
          break;
      }

      foreach (ImportCustomField icf in importFields)
      {
        if (icf.RefType == refType && icf.AuxID == auxID)
        {
          tableName = icf.TableName;
          break;
        }
      }

      if (tableName == "" || fieldName == "") return;
      DataTable table = ReadTable(tableName);
      int count = 0;
      foreach (DataRow row in table.Rows)
      {
        _currentRow = row;
        foreach (ImportCustomField importField in importFields)
        {
          if (importField.TableName == tableName)
          {
            string itemValue = row[importField.FieldName].ToString().Trim();
            if (itemValue != "")
            {
              int id;
              if (idList.TryGetValue(idPrefix + row[fieldName].ToString().Trim().ToLower(), out id))
              {
                  CustomValue value = values.AddNewCustomValue();
                  value.CustomFieldID = importField.TSFieldID;
                  value.Value = itemValue;
                  value.RefID = id;
                try
                {
                  values.Save();
                }
                catch (Exception ex)
                {
                  _log.AppendError(value.Row, ex.Message + ex.StackTrace);
                }

                /*if (++count % BULK_LIMIT == 0)
                {
                  values.BulkSave();
                  values = new CustomValues(_loginUser);
                  GC.WaitForPendingFinalizers();
                }*/

              }
            }
          }
        }
      }

      //values.BulkSave();
      _log.AppendMessage(count.ToString() + " Custom Fields Imported for " + tableName);

    }

    private void ImportCustomFields()
    {

      _importCustomFields = new List<ImportCustomField>();
      CreateAllCustomFields();

      ImportCustomFields(ReferenceType.OrganizationProducts, _importCustomFields, -1);
      ImportCustomFields(ReferenceType.Organizations, _importCustomFields, -1);
      ImportCustomFields(ReferenceType.Products, _importCustomFields, -1);
      ImportCustomFields(ReferenceType.ProductVersions, _importCustomFields, -1);
      ImportCustomFields(ReferenceType.Contacts, _importCustomFields, -1);
      ImportCustomFields(ReferenceType.Users, _importCustomFields, -1);
      ImportCustomFields(ReferenceType.Assets, _importCustomFields, -1);

      TicketTypes ticketTypes = new TicketTypes(_loginUser);
      ticketTypes.LoadAllPositions(_organizationID);
      foreach (TicketType ticketType in ticketTypes)
      {
        ImportCustomFields(ReferenceType.Tickets, _importCustomFields, ticketType.TicketTypeID);
      }


    }

    #endregion

  }
}
