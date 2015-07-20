using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using ENEXWinService.Model;
using Renci.SshNet;
using log4net;

namespace ENEXWinService.Ftp
{
    /*
    public class Sftp : IFtpClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Sftp));
        private static IFtpInfo _ftpInfo;

        public Sftp(IFtpInfo ftpInfo)
        {
            _ftpInfo = ftpInfo;
        }

        public IList<string> GetAllFtpFiles(string remotePath, string regExpression)
        {
            _logger.DebugFormat("RemotePath: {0}", remotePath);
            var fileList = getAllFilePaths(remotePath, regExpression);
            if (fileList == null || !fileList.Any())
                return new List<string>();

            return fileList.Where(file => Regex.IsMatch(file, regExpression)).ToList();
        }

        private string getHost(string host)
        {
            Uri uri = new Uri(host);
            return uri.Host;
        }

        private IList<string> getAllFilePaths(string remotePath, string regExpresion)
        {
            var fileList = new List<string>();

            _logger.DebugFormat("_ftpInfo.Address: {0}", _ftpInfo.Address);
            _logger.DebugFormat(" _ftpInfo.UserName: {0}", _ftpInfo.UserName);
            _logger.DebugFormat("_ftpInfo.PassWord: {0}", _ftpInfo.PassWord);
            _logger.DebugFormat("RemotePath: {0}", remotePath);


            var client = new FtpClient(new FtpInfo(_ftpInfo.Address, _ftpInfo.UserName, _ftpInfo.PassWord));
            fileList= client.GetAllFtpFiles(remotePath, regExpresion).ToList();
                //new SftpClient(getHost(_ftpInfo.Address), 22, _ftpInfo.UserName, _ftpInfo.PassWord);
            //client.Connect();
            //fileList = client.ListDirectory(remotePath).Select(x => x.FullName).ToList();
            //client.Disconnect();
            return fileList;

        }

        public void MoveFtpProcessedFile(string filePath, string processedFilesFtpFolder, string processedFileTextToAppend)
        {
            var movedFile = processedFilesFtpFolder + filePath.Split('/').Last() + processedFileTextToAppend;
            _logger.InfoFormat("Moving file {0} to {1}", filePath, movedFile);
            var client = new FtpClient(new FtpInfo(_ftpInfo.Address, _ftpInfo.UserName, _ftpInfo.PassWord));
            client.MoveFtpProcessedFile(filePath, processedFilesFtpFolder, processedFileTextToAppend);

            //var client = new SftpClient(getHost(_ftpInfo.Address), _ftpInfo.UserName, _ftpInfo.PassWord);
            //client.Connect();
            //client.RenameFile(filePath, movedFile);
            //client.Disconnect();
        }

        public IList<string> ReadLinesFromFtpFile(string file)
        {
            var result = new List<string>();

            var client = new FtpClient(new FtpInfo(_ftpInfo.Address, _ftpInfo.UserName, _ftpInfo.PassWord));
            result = client.ReadLinesFromFtpFile(file).ToList();

            //var client = new SftpClient(getHost(_ftpInfo.Address), _ftpInfo.UserName, _ftpInfo.PassWord);
            //client.Connect();
            //result = client.ReadAllLines(file).ToList();
            //client.Disconnect();
            return result;
        }

    }
     */ 
}
