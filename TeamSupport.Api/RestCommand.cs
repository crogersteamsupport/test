using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using TeamSupport.Data;
using System.IO;

namespace TeamSupport.Api
{
  public enum HttpMethod { Get, Put, Post, Delete, Unsupported }
  public enum RestFormat { XML, XHTML, JSON }

  public class RestCommand
  {
    private HttpStatusCode _statusCode = HttpStatusCode.OK;
    public HttpStatusCode StatusCode
    {
      get { return _statusCode; }
      set { _statusCode = value; }
    }

    private Organization _organization;
    public Organization Organization
    {
      get { return _organization; }
      set { _organization = value; }
    }

    public bool IsCustomerOnly
    {
      get  { return Organization.ParentID != null && Organization.ParentID != 1; }
    }

    private HttpContext _context;
    public HttpContext Context
    {
      get { return _context; }
      set { _context = value; }
    }

    private string _data;
    public string Data
    {
      get { return _data; }
      set { _data = value; }
    }

    private NameValueCollection _filters;
    public NameValueCollection Filters
    {
      get { return _filters; }
      set { _filters = value; }
    }

    private List<string> _segments;
    public List<string> Segments
    {
      get { return _segments; }
      set { _segments = value; }
    }

    private HttpMethod _method;
    public HttpMethod Method
    {
      get { return _method; }
      set { _method = value; }
    }

    private RestFormat _format;
    public RestFormat Format
    {
      get { return _format; }
      set { _format = value; }
    }

    private LoginUser _loginUser;
    public LoginUser LoginUser
    {
      get { return _loginUser; }
      set { _loginUser = value; }
    }

    public RestCommand(LoginUser loginUser, Organization organization, HttpContext context)
    {
      _organization = organization;
      _loginUser = loginUser;

      switch (context.Request.HttpMethod.ToUpper())
      {
        case "GET": _method = HttpMethod.Get; break;
        case "PUT": _method = HttpMethod.Put; break;
        case "POST": _method = HttpMethod.Post; break;
        case "DELETE": _method = HttpMethod.Delete; break;
        default: _method = HttpMethod.Unsupported; break;
      }

      if (_method == HttpMethod.Unsupported)
      {
        throw new RestException(HttpStatusCode.MethodNotAllowed);
      }

      _context = context;
      _segments = new List<string>();
      bool flag = false;
      string format = "";
      for (int i = 0; i < context.Request.Url.Segments.Length; i++)
      {
        string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
        if (flag)
        {
          if (format == "") format = s;
          else _segments.Add(s);
        }

        if (s == "api") flag = true;
      }

      if (_segments == null || _segments.Count < 1)
      {
        throw new RestException(HttpStatusCode.NotFound);
      }

      switch (format)
      {
        case "xml": _format = RestFormat.XML; break;
        case "json": _format = RestFormat.JSON; break;
        case "xhtml": _format = RestFormat.XHTML; break;
        default: _format = RestFormat.XML; break;
      }

      _segments[_segments.Count - 1] = Path.GetFileNameWithoutExtension(_segments[_segments.Count - 1]);


      _filters = context.Request.QueryString;
      _data = ExtractData(_context);
    }


    public string ExtractData(HttpContext context)
    {
      if (context.Request.InputStream != null)
      {
        System.IO.Stream stream = context.Request.InputStream;
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        int length = Convert.ToInt32(stream.Length);
        Byte[] bytes = new Byte[length];
        stream.Read(bytes, 0, length);

        if (bytes.Length > 0)
        {
          return (new UTF32Encoding()).GetString(bytes);
        }
      }
      return "";
    }
  }

}
