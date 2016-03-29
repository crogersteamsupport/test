using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace TeamSupport.JIRA
{
	public class JiraErrorsResponse
	{
		[JsonProperty("errorMessages")]
		public string[] ErrorMessages { get; set; }

		[JsonProperty("errors")]
		public JiraErrors Errors { get; set; }

		public bool HasErrors
		{
			get
			{
				bool hasErrors = (ErrorMessages != null && ErrorMessages.Any())
								|| !string.IsNullOrEmpty(Errors.Issuetype)
								|| !string.IsNullOrEmpty(Errors.Project)
								|| !string.IsNullOrEmpty(Errors.Priority);
				return hasErrors;
			}
		}

		public JiraErrorsResponse()
		{
		}

		public static JiraErrorsResponse Get(WebException webException)
		{
			JiraErrorsResponse errorsResponse = new JiraErrorsResponse();

			StreamReader reader = new StreamReader(webException.Response.GetResponseStream());
			string content = reader.ReadToEnd();
			reader.Close();
			
			errorsResponse = JsonConvert.DeserializeObject<JiraErrorsResponse>(content);

			return errorsResponse;
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();

			if (ErrorMessages != null && ErrorMessages.Any())
			{
				foreach (string errormessage in ErrorMessages)
				{
					if (result.Length > 0)
					{
						result.Append(Environment.NewLine);
					}

					result.Append(errormessage);
				}
			}

			foreach (PropertyInfo propertyInfo in Errors.GetType().GetProperties())
			{
				if (propertyInfo.CanRead)
				{
					if (propertyInfo.GetValue(Errors, null) != null)
					{
						string propertyName = propertyInfo.Name;
						string propertyValue = propertyInfo.GetValue(Errors, null).ToString();

						if (result.Length > 0)
						{
							result.Append(Environment.NewLine);
						}

						result.Append(string.Format("{0}: {1}", propertyName, propertyValue));
					}
				}
			}

			return result.ToString();
		}
	}

	public class JiraErrors
	{

		[JsonProperty("issuetype")]
		public string Issuetype { get; set; }

		[JsonProperty("project")]
		public string Project { get; set; }

		[JsonProperty("priority")]
		public string Priority { get; set; }
	}
}
