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
        LoginUser _loginUser = new LoginUser(ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString, -5, -1, null);

        public Form1()
        {
            InitializeComponent();
        }

        private void start_Click(object sender, EventArgs e)
        {
            SqlCommand loadFilesToMoveCommand = new SqlCommand();
            loadFilesToMoveCommand.CommandText = @"
                SELECT TOP (@attachmentsBatchSize)
                    a.AttachmentID,
                    a.FileName,
                    a.Path
                FROM
                    Attachments a
                    LEFT JOIN FailedToMoveAttachments f
                        ON a.AttachmentID = f.AttachmentID
                WHERE
                    f.AttachmentID IS NULL
                    AND LEFT(a.Path, @sourceLength) = @source
                ORDER BY
                    a.AttachmentID
            ";
            loadFilesToMoveCommand.Parameters.AddWithValue("@attachmentsBatchSize", ConfigurationManager.AppSettings.Get("AttachmentsBatchSize"));
            string source = ConfigurationManager.AppSettings.Get("Source");
            loadFilesToMoveCommand.Parameters.AddWithValue("@source", source);
            loadFilesToMoveCommand.Parameters.AddWithValue("@sourceLength", source.Length);
            DataTable filesToMove = SqlExecutor.ExecuteQuery(_loginUser, loadFilesToMoveCommand);
            string destination = ConfigurationManager.AppSettings.Get("Destination");
            while (filesToMove.Rows.Count > 0)
            {
                foreach (DataRow fileToMove in filesToMove.Rows)
                {
                    try
                    {
                        string fileName = fileToMove[1].ToString();
                        string path = fileToMove[2].ToString();
                        string pathWithoutSource = path.Substring(source.Length, path.Length - source.Length);
                        string pathWithoutSourceAndFile = pathWithoutSource.Replace(fileName, string.Empty);
                        if (!Directory.Exists(destination + pathWithoutSourceAndFile)) Directory.CreateDirectory(destination + pathWithoutSourceAndFile);
                        File.Move(path, destination + pathWithoutSource);
                        SqlCommand updatePathCommand = new SqlCommand();
                        updatePathCommand.CommandText = @"
                            UPDATE
                                Attachments
                            SET
                                Path = @newPath
                            WHERE
                                AttachmentID = @attachmentID
                        ";
                        updatePathCommand.Parameters.AddWithValue("@newPath", destination + pathWithoutSource);
                        updatePathCommand.Parameters.AddWithValue("@attachmentID", (int)fileToMove[0]);
                        SqlExecutor.ExecuteNonQuery(_loginUser, updatePathCommand);
                    }
                    catch (Exception exception)
                    {

                    }
                }
                filesToMove = SqlExecutor.ExecuteQuery(_loginUser, loadFilesToMoveCommand);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private string GetInitialText()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("ConnectionString: " + _loginUser.ConnectionString);
            return result.ToString();
         
        }
    }
}

//namespace TeamSupport.Data
//{
//    public partial class Attachments
//    {
//        public void LoadToMove()
//        {
//            using (SqlCommand command = new SqlCommand())
//            {
//                command.CommandText = "SELECT a.* FROM Attachments a WHERE AttachmentGUID = @attachmentGUID";
//                command.CommandType = CommandType.Text;
//                command.Parameters.AddWithValue("@attachmentGUID", attachmentGUID);
//                BaseCollection.Fill
//            }

//        }
//    }
//}