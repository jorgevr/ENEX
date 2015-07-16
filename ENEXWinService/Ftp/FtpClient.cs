using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ENEXWinService.Model;
using log4net;

namespace ENEXWinService.Ftp
{
    public interface IFtpClient
    {
        IList<string> GetAllFtpFiles(string remotePath, string regExpression);
        void MoveFtpProcessedFile(string filePath, string processedFilesFtpFolder, string processedFileTextToAppend);
        IList<string> ReadLinesFromFtpFile(string file);
    }

    public class FtpClient : IFtpClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(FtpClient));
        private static IFtpInfo _ftpInfo;

        public FtpClient(IFtpInfo ftpInfo)
        {
            _ftpInfo = ftpInfo;
        }

        public IList<string> GetAllFtpFiles(string remotePath, string regExpression)
        {
            var fileList = getAllFilePaths(remotePath);

            if (fileList == null || !fileList.Any())
                return new List<string>();
            // jvr discard files with "OK" termination
            //fileList =  filelist

            return fileList.Where(file => Regex.IsMatch(file, regExpression)).ToList();
        }

        public void MoveFtpProcessedFile(string filePath, string processedFilesFtpFolder, string processedFileTextToAppend)
        {
            var movedFile = processedFilesFtpFolder + filePath.Split('/').Last() + processedFileTextToAppend;
            _logger.InfoFormat("Moving file {0} to {1}", filePath, movedFile);

            var ftpRequest =
                   (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpInfo.Address + filePath));
            ftpRequest.Credentials = new NetworkCredential(_ftpInfo.UserName, _ftpInfo.PassWord);
            ftpRequest.Method = WebRequestMethods.Ftp.Rename;
            ftpRequest.RenameTo = movedFile;

            var response = (FtpWebResponse)ftpRequest.GetResponse();
        }

        public IList<string> ReadLinesFromFtpFile(string file)
        {
            var result = new List<string>();

            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpInfo.Address + file));
            ftpRequest.Credentials = new NetworkCredential(_ftpInfo.UserName, _ftpInfo.PassWord);

            using (WebResponse tmpRes = ftpRequest.GetResponse())
            {
                using (Stream tmpStream = tmpRes.GetResponseStream())
                {
                    using (TextReader tmpReader = new StreamReader(tmpStream))
                    {
                        var line = tmpReader.ReadLine();
                        while (line != null)
                        {
                            result.Add(line);
                            line = tmpReader.ReadLine();
                        }
                    }
                }
            }

            return result;
        }

        private IList<string> getAllFilePaths(string remotePath)
        {
            var fileList = new List<string>();
            var ftpRequest =
                   (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpInfo.Address + remotePath));
            ftpRequest.Credentials = new NetworkCredential(_ftpInfo.UserName, _ftpInfo.PassWord);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

            using (WebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        var line = reader.ReadLine();
                        while (line != null)
                        {
                            fileList.Add(remotePath + line);
                            line = reader.ReadLine();
                        }
                    }
                }
            }

            return fileList;
        }
    }
}
