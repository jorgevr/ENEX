using System;
using System.Collections.Generic;
using System.Linq;
using ENEXWinService.Configuration;
using ENEXWinService.FileExtracter;
using ENEXWinService.Ftp;
using ENEXWinService.Model;
using ENEXWinService.Services;
using log4net;
using Quartz;
using System.IO;

namespace ENEXWinService.Quartz
{
    [DisallowConcurrentExecution]
    public class MeasuresLoaderJob : IJob
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MeasuresLoaderJob));

        private static IConfigurationProvider _configProvider;
        private static IMeasureService _measureService;
        private static IPlantPowerService _plantPowerService;
        private static IPlantService _plantService;
        private static IMeasureFileExtracter _fileExtracter;
        private static IPlantPowerFileExtracter _filePlantPowerExtracter;
        private static IFtpClient _ftpClient;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Info("FileProcesserJob started");
                execute(context);
                _logger.Info("FileProcesserJob ended");
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Exception while executing FileProcesserJob: {0}\nStackTrace: \n{1}\n", e.Message, e.StackTrace);
            }
        }

        private void execute(IJobExecutionContext context)
        {
            //jvr breAKpoint
            prepareContextData(context);
            refreshConfigSettings();
            //var plant = getPlant();
            // jvr we process all files with .txt termination
            // var processFilesList=getProcessedMeasuresFilesList();
            var filePaths = getNoProcessedFtpFiles();//processFilesList);


            if (filePaths == null || !filePaths.Any())
            {
                _logger.Warn("There are no files to process.");
                return;
            }
            _logger.InfoFormat("Found {0} files to process", filePaths.Count);
            processFiles(filePaths);//, plant);
        }

        /* private ApiPlant getPlant()
         {
             return _plantService.GetPlant(_configProvider.GetHourlyPlantsString());
         }*/

        private IList<String> getNoProcessedFtpFiles()       // IList<string> processFilesList)
        {
            IList<String> noProcessedFilesList = new List<String>();
            var allFilePathsList = getAllFtpFiles();

            //jvr this loop is not neccesary as all *.txt files are processed 
            /*
            foreach (String file in allFilePathsList) 
            {
                _logger.DebugFormat("Process File List have {0} files", processFilesList.Count);
                if (!processFilesList.Any(x => x == new FileInfo(file).Name))
                {
                    noProcessedFilesList.Add(file);
                    _logger.DebugFormat("New detected File: {0}", file);
                }
                else 
                {
                    _logger.DebugFormat("File is processed: {0}", file);
                }
            }*/
            // return noProcessedFilesList;
            return allFilePathsList;
        }

        private IList<String> getProcessedMeasuresFilesList()
        {
            var processFileListPath = _configProvider.GetProcessMeasuresFilesListPath();
            return readProcessedfilesFromTxt(processFileListPath);

        }

        private IList<string> readProcessedfilesFromTxt(string processFileListPath)
        {
            List<String> fileList = new List<String>();
            using (StreamReader sr = new StreamReader(processFileListPath))
            {
                while (!sr.EndOfStream)
                {
                    string file = sr.ReadLine();
                    fileList.Add(file);
                }
                return fileList.OrderBy(x => x).ToList();
            }
        }

        private static void updateCurrentFileListToTxt(string processFileListPath, string updateFile)
        {
            string tempfile = Path.GetTempFileName();
            StreamWriter writer = new StreamWriter(tempfile);
            StreamReader reader = new StreamReader(processFileListPath);
            writer.WriteLine(new FileInfo(updateFile).Name);
            while (!reader.EndOfStream)
                writer.WriteLine(reader.ReadLine());
            writer.Close();
            reader.Close();
            File.Copy(tempfile, processFileListPath, true);
        }



        private IList<String> getAllFtpFiles()
        {
            var remotePath = _configProvider.GetFtpRemotePath();
            var regExpression = _configProvider.GetFtpRegExpression();
            return _ftpClient.GetAllFtpFiles(remotePath, regExpression).OrderBy(x => x).ToList();
        }

        private void prepareContextData(IJobExecutionContext context)
        {
            _logger.Info("Getting Job Data from context");

            var confProvider = context.Trigger.JobDataMap.Get("ConfigurationProvider") as IConfigurationProvider;
            if (confProvider == null)
                throw new ArgumentException("Cant get IConfigurationProvider from context");
            _configProvider = confProvider;

            var ftpClient = context.Trigger.JobDataMap.Get("FtpClient") as IFtpClient;
            if (ftpClient == null)
                throw new ArgumentException("Cant get IFtpClient from context");
            _ftpClient = ftpClient;

            var service = context.Trigger.JobDataMap.Get("MeasureService") as IMeasureService;
            if (service == null)
                throw new ArgumentException("Cant get IMeasureService from context");
            _measureService = service;

            var plantPowerService = context.Trigger.JobDataMap.Get("PlantPowerService") as IPlantPowerService;
            if (plantPowerService == null)
                throw new ArgumentException("Cant get IPlantPowerService from context");
            _plantPowerService = plantPowerService;

            var plantService = context.Trigger.JobDataMap.Get("PlantService") as IPlantService;
            if (plantService == null)
                throw new ArgumentException("Cant get IPlantService from context");
            _plantService = plantService;

            var extracter = context.Trigger.JobDataMap.Get("MeasureFileExtracter") as IMeasureFileExtracter;
            if (extracter == null)
                throw new ArgumentException("Cant get IMeasureFileExtracter from context");
            _fileExtracter = extracter;

            var extracterPlantPower = context.Trigger.JobDataMap.Get("PlantPowerFileExtracter") as IPlantPowerFileExtracter;
            if (extracterPlantPower == null)
                throw new ArgumentException("Cant get IPlantPowerFileExtracter from context");
            _filePlantPowerExtracter = extracterPlantPower;

            _logger.Info("Done getting Job Data from context");
        }

        private void processFiles(IList<string> filePaths) //, ApiPlant plant)
        {
            foreach (var filePath in filePaths)
            {
                _logger.Debug(Path.GetFileName(filePath));
                string checkPlant = _fileExtracter.getPlantName(Path.GetFileName(filePath));
                // jvr: skip file when name of the plant is not recognized
                if (checkPlant == "unknown")
                {
                    _logger.InfoFormat("Unkown name of the plant: {0}", Path.GetFileName(filePath));
                    continue;
                }

                var plant = _plantService.GetPlant(checkPlant);
                try
                {
                    _logger.InfoFormat("Processing file: {0}", filePath);
                    processFile(filePath, plant);
                    _logger.InfoFormat("Processed file: {0}", filePath);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Exception while processing file: {0}\nStackTrace: \n{1}\n", e.Message, e.StackTrace);
                }
            }
        }

        private void processFile(string filePath, ApiPlant plant)
        {
            var measures = getAllMeasuresFromFile(filePath, plant);
            //var plantPower=updatePlantPowerFromFile(filePath, plant);
            if (measures == null || !measures.Any())
            {
                _logger.Warn("No measures found inside the file");
                // jvr we check as processed even when no data is found
                //AQUI
                var processedFileTextToAppend = _configProvider.GetFtpProcessedFilesPath();
                _ftpClient.MoveFtpProcessedFile(filePath, processedFileTextToAppend, ".nodata");
                return;
            }
            _logger.InfoFormat("Found {0} measures to send", measures.Count);
            sendMeasures(filePath, measures);
            /*if (plantPower == null || !plantPower.Any())
            {
                _logger.Warn("No change Power found inside the file");
                return;
            }*/
            //_logger.InfoFormat("Found {0} change power to update", plantPower.Count);        

        }

        private IList<PlantPower> updatePlantPowerFromFile(string filePath, ApiPlant plant)
        {
            var fileLines = _ftpClient.ReadLinesFromFtpFile(filePath);
            if (fileLines == null || !fileLines.Any())
                return new List<PlantPower>();

            var fileName = filePath.Split('/').Last();
            var result = new List<PlantPower>();
            var contLine = 0;
            foreach (var line in fileLines)
            {
                if (contLine != 0)//cabecera
                {
                    var plantPowerFile = _filePlantPowerExtracter.ProcessLine(line, fileName, plant);
                    var updatENEXlantPower = updatePlantPower(_plantPowerService, plantPowerFile, filePath);
                    if (updatENEXlantPower != null)
                    {
                        _logger.DebugFormat("Updating PlantPower Date: {0} Power:{1} from file {2}", updatENEXlantPower.UtcUpdatedDateTime, updatENEXlantPower.Power, filePath);
                        result.Add(updatENEXlantPower);
                    }
                }
                contLine++;
            }
            return result;



        }

        private PlantPower updatePlantPower(IPlantPowerService _plantPowerService, PlantPower plantPowerFile, string filePath)
        {
            var plantPowerList = _plantPowerService.GetPlantPower(_configProvider.GetHourlyPlantsString()).OrderByDescending(x => x.UtcUpdatedDateTime);
            var currentPlantPower = plantPowerList.FirstOrDefault(x => Double.Parse(x.UtcUpdatedDateTime) < Double.Parse(plantPowerFile.UtcUpdatedDateTime));
            if (plantPowerFile.Power != currentPlantPower.Power)
            {
                _logger.InfoFormat("Updating PlantPower from File, Old PlantPower:{0}kW,Update:{1},Insert:{2}-> New PlantPower:{3}kW,Update:{4},Insert:{5}",
               currentPlantPower.Power, currentPlantPower.UtcUpdatedDateTime, currentPlantPower.UtcInsertionDateTime,
               plantPowerFile.Power, plantPowerFile.UtcUpdatedDateTime, plantPowerFile.UtcInsertionDateTime);
                sendPlantPower(plantPowerFile, filePath);
                return plantPowerFile;
            }
            else
            {
                return null;
            }


        }

        private IList<Measure> getAllMeasuresFromFile(string filePath, ApiPlant plant)
        {
            var fileLines = _ftpClient.ReadLinesFromFtpFile(filePath);
            if (fileLines == null || !fileLines.Any())
                return new List<Measure>();

            var fileName = filePath.Split('/').Last();
            var result = new List<Measure>();
            var contLine = 0;
            foreach (var line in fileLines)
            {
                if (contLine != 0)//cabecera
                {
                    _logger.Debug(line);
                    var measure = _fileExtracter.ProcessLine(line, Path.GetFileName(fileName), plant);
                    if (measure != null)
                        result.Add(measure);
                }
                contLine++;
            }
            return result;
        }

        private void sendMeasures(string filePath, IList<Measure> measures)
        {
            trySendMeasures(filePath, measures);
            //JVR updateCurrentFileList(filePath);
        }

        private void sendPlantPower(PlantPower plantPower, string filePath)
        {
            trySendPlantPower(plantPower, filePath);
        }

        private bool trySendPlantPower(PlantPower plantPower, string filePath)
        {
            if (!_plantPowerService.TrySendPlantPower(plantPower))
            {
                _logger.ErrorFormat("PlantPower won't be updated from : {0}", filePath);
                return false;
            }
            _logger.InfoFormat("DONE sending PlantPower Date: {0} Power:{1} from file {2}", plantPower.UtcUpdatedDateTime, plantPower.Power, filePath);
            return true;
        }

        private void updateCurrentFileList(string filePath)
        {
            _logger.Info("Update processed measure list file");
            var processedFilesTxtPath = _configProvider.GetProcessMeasuresFilesListPath();
            tryUpdateCurrentFileListToTxt(processedFilesTxtPath, filePath);
        }


        private bool tryUpdateCurrentFileListToTxt(string processedFilesTxtPath, string filePath)
        {
            _logger.InfoFormat("Updating {0} file to file {1}", filePath, processedFilesTxtPath);
            try
            {
                updateCurrentFileListToTxt(processedFilesTxtPath, filePath);
                _logger.InfoFormat("DONE Updating {0} file to file {1}", filePath, processedFilesTxtPath);
                return true;
            }
            catch
            {
                _logger.ErrorFormat("The file {0} won't be updated to file: {1}", filePath, processedFilesTxtPath);
                return false;
            }
        }

        private bool trySendMeasures(string filePath, IList<Measure> measures)
        {
            _logger.InfoFormat("Sending {0} Measures from file {1}", measures.Count, filePath);
            if (!_measureService.TrySendMeasures(measures))
            {
                _logger.ErrorFormat("The file won't be moved from its actual location: {0}", filePath);
                return false;
            }

            moveProcessedFile(filePath);
            _logger.InfoFormat("DONE sending {0} Measures from file {1}", measures.Count, filePath);
            return true;
        }

        private void moveProcessedFile(string filePath)
        {
            _logger.Info("Moving file to new location");
            var processedFilesFtpFolder = _configProvider.GetFtpProcessedFilesPath();
            var processedFileTextToAppend = _configProvider.GetFtpProcessedFileTextToAppend();
            _ftpClient.MoveFtpProcessedFile(filePath, processedFilesFtpFolder, processedFileTextToAppend);
            _logger.Info("DONE moving file to new location");
        }


        private void refreshConfigSettings()
        {
            _logger.Debug("Refreshing config file cache");
            _configProvider.Refresh();
            _logger.Debug("Done refreshing config file cache");
        }
    }
}
