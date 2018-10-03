using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Data;

namespace TeamSupport.Data.Quarantine
{
    public class ServiceQ
    {
        public static string UpdloadAttachments(LoginUser _loginUser, CRMLinkErrors _crmErrors, TeamSupport.ServiceLibrary.Log _log, Data.Action action, string encodedCredentials, string result, string appId, string hostName)
        {
            Attachments attachments = new Attachments(_loginUser);
            attachments.LoadForIntegration(action.ActionID, IntegrationType.ServiceNow);

            foreach (Attachment attachment in attachments)
            {
                UploadAttachment(_loginUser, _crmErrors, _log, appId, encodedCredentials, ref result, attachment, hostName);
            }

            return result;
        }

        private static HttpStatusCode UploadAttachment(LoginUser _loginUser, CRMLinkErrors _crmErrors, TeamSupport.ServiceLibrary.Log _log, string incidentId, string encodedCredentials, ref string responseString, Attachment attachment, string hostName)
        {
            HttpStatusCode resultStatusCode = HttpStatusCode.BadRequest;
            string error = string.Empty;

            if (File.Exists(attachment.Path))
            {
                FileStream attachmentFileStream = new FileStream(attachment.Path, FileMode.Open, FileAccess.Read);
                int fileSizeLimit = 50000000; //vv 50MB. can we get this from Snow?

                if (attachmentFileStream.Length <= fileSizeLimit)
                {
                    try
                    {
                        string url = GetFullUrl(hostName, string.Format("api/now/attachment/file?table_name=incident&table_sys_id={0}&file_name={1}", incidentId, System.Web.HttpUtility.UrlEncode(attachment.FileName)));
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.Method = "POST";
                        request.ContentType = "multipart/form-data";
                        request.Accept = "application/json";
                        request.Headers.Add("Authorization", "Basic " + encodedCredentials);

                        MemoryStream content = new MemoryStream();
                        StreamWriter writer = new StreamWriter(content);
                        byte[] data = new byte[attachmentFileStream.Length + 1];
                        attachmentFileStream.Read(data, 0, data.Length);
                        attachmentFileStream.Close();
                        content.Write(data, 0, data.Length);
                        writer.WriteLine();
                        writer.Flush();
                        content.Seek(0, SeekOrigin.Begin);
                        request.ContentLength = content.Length;

                        using (Stream requestStream = request.GetRequestStream())
                        {
                            content.WriteTo(requestStream);
                            requestStream.Close();
                        }

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            _log.Write("Attachment \"" + attachment.FileName + "\" sent.");
                            Stream responseStream = response.GetResponseStream();

                            if (responseStream != null)
                            {
                                var streamReader = new StreamReader(responseStream);
                                responseString = streamReader.ReadToEnd();
                            }

                            resultStatusCode = response.StatusCode;
                            response.Close();
                        }

                        content.Flush();
                        content.Close();
                        attachment.SentToSnow = true;
                        attachment.Collection.Save();
                        _log.ClearCrmLinkError(attachment.AttachmentID.ToString(), string.Empty, _crmErrors);
                    }
                    catch (WebException webEx)
                    {
                        using (var response = (HttpWebResponse)webEx.Response)
                        {
                            Stream responseStream = response.GetResponseStream();

                            if (responseStream != null)
                            {
                                var reader = new StreamReader(responseStream);
                                string responseText = string.Format("Error code: {0} - {1}{2}{3}", response.StatusCode, response.StatusDescription, Environment.NewLine, reader.ReadToEnd());
                                throw new Exception(responseText);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                }
                else
                {
                    error = string.Format("Attachment was not sent as its file size ({0}) exceeded the file size limit of {1}", attachmentFileStream.Length.ToString(), fileSizeLimit.ToString());
                }
            }
            else
            {
                error = "Attachment was not sent as it was not found on server";
            }

            if (!string.IsNullOrEmpty(error))
            {
                _crmErrors.LoadByOperationAndObjectId(attachment.OrganizationID, Enums.GetDescription(IntegrationType.ServiceNow), Enums.GetDescription(IntegrationOrientation.OutToServiceNow), "attachment", attachment.AttachmentID.ToString());
                _log.WriteToCrmErrorReport(_loginUser, error, attachment.OrganizationID, attachment.AttachmentID.ToString(), "attachment", "file", attachment.FileName, "create", IntegrationType.ServiceNow, Enums.GetDescription(IntegrationOrientation.OutToServiceNow), _crmErrors, logText: true);
            }

            return resultStatusCode;
        }

        private static string GetFullUrl(string hostName, string path)
        {
            if (!hostName.EndsWith("/"))
            {
                hostName += "/";
            }

            return hostName + path;
        }

        /*
TeamSupport.Data.Quarantine.ServiceQ
    */
    }

    //There are at this point AT LEAST two places with the following Log class duplicated (HubSpotSources/SyncLog.cs) and other more with something similar. We need to centralize this at some point!
    public class Log
    {
        private string _logPath;
        private string _fileName;

        public Log(string path, string logType, int? threadNumber = null)
        {
            _logPath = path;
            _fileName = string.Format("{0} Debug File {1} - {2}{3}{4}.txt", logType, (threadNumber == null ? "" : "[" + threadNumber.ToString() + "]"), DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Day.ToString(), DateTime.UtcNow.Year.ToString());

            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
        }

        public void Write(string text)
        {
            //the very first time we write to this file (once each day), purge old files
            if (!File.Exists(string.Format("{0}\\{1}", _logPath, _fileName)))
            {
                foreach (string oldFileName in Directory.GetFiles(_logPath))
                {
                    if (File.GetLastWriteTime(oldFileName).AddDays(7) < DateTime.UtcNow)
                    {
                        File.Delete(oldFileName);
                    }
                }
            }

            File.AppendAllText(string.Format("{0}\\{1}", _logPath, _fileName), string.Format("{0}: {1}{2}", DateTime.Now.ToLongTimeString(), text, Environment.NewLine));
        }

        public void WriteData(DataRow row)
        {
            Write("Data Row:");
            Write(DataUtils.DataRowToString(row));
        }

        public void WriteToCrmErrorReport(LoginUser loginUser,
                                        string errorMessage,
                                        int organizationId,
                                        string objectId,
                                        string objectType,
                                        string objectFieldName,
                                        string objectData,
                                        string operation,
                                        IntegrationType integration,
                                        string orientation,
                                        CRMLinkErrors crmErrors,
                                        bool logText = false)
        {
            CRMLinkError error = crmErrors.FindUnclearedByObjectIDAndFieldName(objectId, objectFieldName);

            if (error == null)
            {
                CRMLinkErrors newCrmLinkError = new CRMLinkErrors(loginUser);
                error = newCrmLinkError.AddNewCRMLinkError();
                error.OrganizationID = organizationId;
                error.CRMType = Enums.GetDescription(integration);
                error.Orientation = orientation;
                error.ObjectType = objectType;
                error.ObjectID = objectId.ToString();
                error.ObjectFieldName = objectFieldName;
                error.ObjectData = objectData;
                error.Exception = errorMessage;
                error.OperationType = operation;
                error.ErrorCount = 1;
                error.IsCleared = false;
                error.ErrorMessage = errorMessage;
                newCrmLinkError.Save();
            }
            else
            {
                error.ErrorCount = error.ErrorCount + 1;
                error.IsCleared = false;
                error.ObjectData = objectData;
                error.Exception = errorMessage;
                error.Collection.Save();
            }

            if (logText)
            {
                Write(errorMessage);
            }
        }

        public void ClearCrmLinkError(string objectId, string objectFieldName, CRMLinkErrors crmErrors)
        {
            CRMLinkError error = crmErrors.FindUnclearedByObjectIDAndFieldName(objectId, objectFieldName);

            if (error != null)
            {
                error.IsCleared = true;
                error.DateModified = DateTime.UtcNow;
                error.Collection.Save();
            }
        }
    }
}
