using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace FilesMover
{
    public partial class Form1 : Form
    {
        private LoginUser _loginUser = new LoginUser(ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString, -5, -1, null);
        private Logs _logs = new Logs(DateTime.Now.ToString("HH-mm"));
        private int _attachmentsBatchSize = Int32.Parse(ConfigurationManager.AppSettings.Get("AttachmentsBatchSize"));
        private string _source = ConfigurationManager.AppSettings.Get("Source");
        private string _destination = ConfigurationManager.AppSettings.Get("Destination");
        private int _filesMoved = 0;
        private bool _stop = false;

        public Form1()
        {
            InitializeComponent();
            DisplayAndLog(GetInitialText());
        }

        private void start_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayAndLog("Start at " + DateTime.Now.ToString());
                DataTable filesToMove = GetFilesToMove(_attachmentsBatchSize);
                DisplayAndLog("Got batch of " + filesToMove.Rows.Count.ToString() + " files.");
                while (filesToMove.Rows.Count > 0 && !_stop)
                {
                    foreach (DataRow fileToMove in filesToMove.Rows)
                    {
                        try
                        {
                            if (!_stop)
                            {
                                string pathWithoutSource = MoveFile(fileToMove);
                                UpdatePath((int)fileToMove[0], pathWithoutSource);
                                _filesMoved++;
                                DisplayAndLog("Moved file with AttachmentID: " + fileToMove[0].ToString());
                            }
                        }
                        catch (Exception exception)
                        {
                            AddFailedMove((int)fileToMove[0], exception);
                            DisplayAndLog("Moving file with AttachmentID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                        }
                    }
                    filesToMove = GetFilesToMove(_attachmentsBatchSize);
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void DisplayAndLog(string message)
        {
            string messageWithCount = "#: " + _filesMoved.ToString() + " " + message;
            textBox1.AppendText(messageWithCount + Environment.NewLine);
            _logs.WriteEvent(messageWithCount);
        }

        private string GetInitialText()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("ConnectionString: " + _loginUser.ConnectionString);
            result.AppendLine("AttachmentsBatchSize: " + _attachmentsBatchSize.ToString());
            result.AppendLine("Source: " + _source);
            result.AppendLine("Destination: " + _destination);
            return result.ToString();
        }

        private DataTable GetFilesToMove(int attachmentsBatchSize)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "LoadMoveAttachmentsQuery";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@attachmentsBatchSize", attachmentsBatchSize);
            command.Parameters.AddWithValue("@source", _source);
            //command.Parameters.AddWithValue("@sourceLength", _source.Length);
            return SqlExecutor.ExecuteQuery(_loginUser, command);
        }

        private string MoveFile(DataRow fileToMove)
        {
            string fileName = fileToMove[1].ToString();
            string path = fileToMove[2].ToString();
            string pathWithoutSource = path.Substring(_source.Length, path.Length - _source.Length);
            string pathWithoutSourceAndFile = pathWithoutSource.Replace(fileName, string.Empty);
            if (!Directory.Exists(_destination + pathWithoutSourceAndFile)) Directory.CreateDirectory(_destination + pathWithoutSourceAndFile);
            File.Move(path, _destination + pathWithoutSource);
            return pathWithoutSource;
        }

        private void UpdatePath(int attachmentID, string pathWithoutSource)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                UPDATE
                    Attachments
                SET
                    Path = @newPath
                WHERE
                    AttachmentID = @attachmentID
            ";
            command.Parameters.AddWithValue("@newPath", _destination + pathWithoutSource);
            command.Parameters.AddWithValue("@attachmentID", attachmentID);
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }


        private void AddFailedMove(int attachmentID, Exception ex)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
                INSERT INTO
                    FailedToMoveAttachments
                VALUES
                (
                    @attachmentID,
                    @exceptionMessage,
                    @exceptionType
                )
            ";
            command.Parameters.AddWithValue("@attachmentID", attachmentID);
            command.Parameters.AddWithValue("@exceptionMessage", ex.Message);
            command.Parameters.AddWithValue("@exceptionType", ex.GetType().ToString());
            SqlExecutor.ExecuteNonQuery(_loginUser, command);
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void step_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayAndLog("Start at " + DateTime.Now.ToString());
                DataTable filesToMove = GetFilesToMove(1);
                DisplayAndLog("Got batch of " + filesToMove.Rows.Count.ToString() + " files.");
                foreach (DataRow fileToMove in filesToMove.Rows)
                {
                    try
                    {
                        string pathWithoutSource = MoveFile(fileToMove);
                        UpdatePath((int)fileToMove[0], pathWithoutSource);
                        _filesMoved++;
                        DisplayAndLog("Moved file with AttachmentID: " + fileToMove[0].ToString());
                    }
                    catch (Exception exception)
                    {
                        AddFailedMove((int)fileToMove[0], exception);
                        DisplayAndLog("Moving file with AttachmentID: " + fileToMove[0].ToString() + " got exception: " + exception.Message);
                    }
                }
            }
            catch (Exception exception)
            {
                DisplayAndLog(exception.Message);
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            _stop = true;
        }
    }
}