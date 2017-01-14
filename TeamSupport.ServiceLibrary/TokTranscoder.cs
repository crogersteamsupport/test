using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using Ionic.Zip;
using System.Diagnostics;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Specialized;

namespace TeamSupport.ServiceLibrary
{
    [Serializable]
    public class TokTranscoder : ServiceThreadPoolProcess
    {
        private static object _staticLock = new object();
        //value for the current bucket to use
        string bucketName = "";
        //path of the file that is being referrenced
        string s3Path = "";
        //value for the filename to use when uploading
        string uploadS3Path = "";
        //path of the unzipped files
        string outputFileLocation = "";
        //path to download the zip files
        string downloadLocation = "";
        string ffmpegPath = "";
        string archiveID;

        static List<string> webmFiles = new List<string>();
        static IAmazonS3 client;

        public override void ReleaseAllLocks()
        {
            //You need to lock this, if you plan on multiple threads
        }

        public override void Run()
        {
            Logs.WriteEvent("Starting Run");
            UpdateHealth();

            TokStorage ts = new TokStorage(LoginUser);
            ts.GetNonTranscoded();

            foreach (var t in ts)
            {
                Logs.WriteEvent(t.AmazonPath);
                InitSettings(t.AmazonPath);
                using (client = new AmazonS3Client())
                {
                    DownLoadZip();
                    ExtractZip($@"{downloadLocation}/{s3Path}");
                    MergeVideoFiles();
                    UploadHighResVideo();
                    CleanUpFiles();
                }
                UpdateHealth();
            }


        }


        void InitSettings(string amazonPath)
        {
            Logs.WriteEvent("----- Building Configuration Settings");
            string[] path = amazonPath.Split('/');
            bucketName = path[3];
            archiveID = path[5];
            s3Path = ($@"{path[4]}/{path[5]}/archive.zip");
            uploadS3Path = ($@"{path[4]}/{path[5]}/archive.mp4");
            outputFileLocation = Settings.ReadString("outputFilePath");
            downloadLocation = Settings.ReadString("downloadFilePath");
            ffmpegPath = Settings.ReadString("ffmpegPath");

        }

        // High Res Video Process
        // Download zip of webm files
        // Extract files
        // Merge webm video files
        // Upload merged file to amazon s3
        // Delete all related files


        void DownLoadZip()
        {
            Logs.WriteEvent("----- Downloading Zip ...");
            try
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = s3Path
                };

                using (GetObjectResponse response = client.GetObject(request))
                {
                    string dest = Path.Combine(downloadLocation, s3Path);
                    if (!File.Exists(dest))
                    {
                        response.WriteResponseStreamToFile(dest);
                    }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Logs.WriteEvent("Please check the provided AWS Credentials.");
                    Logs.WriteEvent("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Logs.WriteEvent(string.Format("An error occurred with the message '{0}' when reading an object", amazonS3Exception.Message));
                }
            }
        }

        void ExtractZip(string path)
        {
            Logs.WriteEvent("----- Extracting Zip ...");
            string activeDirectory = Path.GetDirectoryName(path);

            using (ZipFile zip1 = ZipFile.Read(path))
            {
                foreach (ZipEntry e in zip1.OrderBy(e=>e.UncompressedSize))
                {
                    if (Path.GetExtension(e.FileName) == ".webm")
                    {
                        e.Extract(Path.GetDirectoryName(path), ExtractExistingFileAction.OverwriteSilently);
                        webmFiles.Add(Path.Combine(activeDirectory, e.FileName));
                    }
                }
            }
        }

        void MergeVideoFiles()
        {
            Logs.WriteEvent("----- Merging webm files ...");
            outputFileLocation = Path.Combine(Path.GetDirectoryName(webmFiles[0]), "archive.mp4");
            Process proc1 = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(ffmpegPath, "ffprobe.exe"),
                    Arguments = $@"-v error -show_frames -of default=noprint_wrappers=1 {webmFiles[1]}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };


            if (!proc1.Start())
            {
                Logs.WriteEvent("Error starting");
                return;
            }
            StreamReader rw = proc1.StandardOutput;
            var height = "";
            var width = "";
            string line;
            while ((line = rw.ReadLine()) != null)
            {
                if (line.Contains("width"))
                {
                    width = line.Split('=')[1];
                }
                if (line.Contains("height"))
                {
                    height = line.Split('=')[1];
                    break;
                }
            }
            //proc1.Close();
            proc1.Kill();

            Process proc = new Process();
            proc.StartInfo.FileName = Path.Combine(ffmpegPath,"ffmpeg.exe");
            proc.StartInfo.Arguments = $@"-i {webmFiles[0]} -i {webmFiles[1]} -map 0:0 -map 1:1 -codec:a aac -ab 128k -codec:v libx264 -vf scale={width}:{height} -aspect 16:9 -r 30 {outputFileLocation}";
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.UseShellExecute = false;

            if (!proc.Start())
            {
                Logs.WriteEvent("Error starting");
                return;
            }
            StreamReader reader = proc.StandardError;
            
            while ((line = reader.ReadLine()) != null)
            {
                Logs.WriteEvent(line);
            }
            proc.Close();
        }

        void UploadHighResVideo()
        {
            Logs.WriteEvent("----- Uploading Converted Video ...");
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    FilePath = outputFileLocation,
                    BucketName = bucketName,
                    Key = uploadS3Path
                };

                PutObjectResponse response = client.PutObject(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Logs.WriteEvent("Please check the provided AWS Credentials.");
                    Logs.WriteEvent("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    Logs.WriteEvent(string.Format("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message));
                }
            }
        }

        void CleanUpFiles()
        {
            Logs.WriteEvent("----- Cleaning up files and marking as processed");
            string dest = Path.GetDirectoryName(webmFiles[0]);
            System.IO.DirectoryInfo di = new DirectoryInfo(dest);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            di.Delete();
            webmFiles.Clear();

            TokStorage dbItem = new TokStorage(LoginUser);

            dbItem.LoadByArchiveID(archiveID);
            dbItem[0].Transcoded = true;
            dbItem[0].Collection.Save();

        }

    }
}
