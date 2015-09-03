using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(ImportFieldProxy))]
  public class ImportFieldProxy
  {
    public ImportFieldProxy() {}
    [DataMember] public int ImportFieldID { get; set; }
    [DataMember] public string TableName { get; set; }
    [DataMember] public string FieldName { get; set; }
    [DataMember] public string Alias { get; set; }
    [DataMember] public string DataType { get; set; }
    [DataMember] public int Size { get; set; }
    [DataMember] public bool IsVisible { get; set; }
    [DataMember] public bool IsRequired { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public bool Enabled { get; set; }
    [DataMember] public int Position { get; set; }
          
  }
  
  public partial class ImportField : BaseItem
  {
    public ImportFieldProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ImportFieldProxy result = new ImportFieldProxy();
      result.Position = this.Position;
      result.Enabled = this.Enabled;
      result.RefType = this.RefType;
      result.Description = sanitizer.Sanitize(this.Description);
      result.IsRequired = this.IsRequired;
      result.IsVisible = this.IsVisible;
      result.Size = this.Size;
      result.DataType = sanitizer.Sanitize(this.DataType);
      result.Alias = sanitizer.Sanitize(this.Alias);
      result.FieldName = sanitizer.Sanitize(this.FieldName);
      result.TableName = sanitizer.Sanitize(this.TableName);
      result.ImportFieldID = this.ImportFieldID;
       
       
       
      return result;
    }	
  }
}
