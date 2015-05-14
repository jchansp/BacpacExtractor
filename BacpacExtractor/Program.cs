using System;
using System.Net;
using Microsoft.SqlServer.Dac;

namespace BacpacExtractor
{
    internal class Program
    {
        //private const string TargetConnectionString = "Server=.;Database=QA-EduSistema-20150512200000;Trusted_Connection=True;";
        private const string TargetConnectionString = "Server=.;Trusted_Connection=True;";
        private const string TargetDatabaseName = "QA-EduSistema-20150513200001";

        private const string RemoteUri =
            "http://eduapp.edumod2.net/Subscripciones/DatosColegios/DescargarBackup?nombreArchivo=";

        private const string FileName = "QA-EduSistema-20150513200001";
        private const string SqlPackagePath = @"C:\Program Files (x86)\Microsoft SQL Server\120\DAC\bin";

        private static void Main(string[] args)
        {
            DownloadFile();
            ExtractBacpac();
        }

        private static void DownloadFile()
        {
            const string webResource = RemoteUri + TargetDatabaseName + ".bacpac";
            var webClient = new WebClient();
            webClient.DownloadFile(webResource, FileName + ".bacpac");
            Console.WriteLine("Successfully Downloaded File \"{0}.bacpac\" from \"{1}\"", TargetDatabaseName, webResource);
        }

        private static void ExtractBacpac()
        {
            var startTime = DateTime.Now;
            try
            {
                var svc = new DacServices(TargetConnectionString);
                svc.Message += ReceiveDacServiceMessageEvent;
                svc.ProgressChanged += ReceiveDacServiceProgessEvent;
                Import(svc);
            }
            catch (DacServicesException e)
            {
                Console.WriteLine("Error Encountered:{0} Inner Exception: {1}", e.Messages, e.InnerException);
            }
            finally
            {
                Console.WriteLine("Tests completed at {0} in {1} minutes {2} seconds {3} milliseconds",
                    DateTime.Now.ToLongTimeString(), DateTime.Now.Subtract(startTime).Minutes,
                    DateTime.Now.Subtract(startTime).Seconds, DateTime.Now.Subtract(startTime).Milliseconds);
                Console.WriteLine("\n\rStrike key to end execution");
                Console.ReadKey();
            }
        }

        #region DAC Methods

        private static void Import(DacServices svc)
        {
            const string path = TargetDatabaseName + ".bacpac";
            Console.WriteLine("\n\rPerforming Import of {0} to {1} at {2}", path, TargetDatabaseName,
                DateTime.Now.ToLongTimeString());

            using (var bacpac = BacPackage.Load(path))
            {
                svc.ImportBacpac(bacpac, TargetDatabaseName);
            }
        }

        private static void Extract(DacServices svc, string sourceDatabaseName, string path)
        {
            Console.WriteLine("\n\rPerforming Extract of {0} to {1} at {2}", sourceDatabaseName, path,
                DateTime.Now.ToLongTimeString());

            var dacExtractOptions = new DacExtractOptions
            {
                ExtractApplicationScopedObjectsOnly = true,
                ExtractReferencedServerScopedElements = false,
                VerifyExtraction = true,
                Storage = DacSchemaModelStorageType.Memory
            };

            svc.Extract(path, sourceDatabaseName, "Sample DACPAC", new Version(1, 0, 0), "Sample Extract", null,
                dacExtractOptions);
        }

        private static void Deploy(DacServices svc, string targetDatabaseName, string path)
        {
            Console.WriteLine("\n\rPerforming Deploy of {0} to {1} at {2}", path, targetDatabaseName,
                DateTime.Now.ToLongTimeString());

            using (var dacpac = DacPackage.Load(path))
            {
                svc.Deploy(dacpac, targetDatabaseName);
            }
        }

        private static void Export(DacServices svc, string sourceDatabaseName, string path)
        {
            Console.WriteLine("\n\rPerforming Export of {0} to {1} at {2}", sourceDatabaseName, path,
                DateTime.Now.ToLongTimeString());

            svc.ExportBacpac(path, sourceDatabaseName);
        }


        [Obsolete]
        private static void Import(DacServices svc, string targetDatabaseName, string path)
        {
            path = targetDatabaseName + ".bacpac";
            Console.WriteLine("\n\rPerforming Import of {0} to {1} at {2}", path, targetDatabaseName,
                DateTime.Now.ToLongTimeString());

            using (var bacpac = BacPackage.Load(path))
            {
                svc.ImportBacpac(bacpac, targetDatabaseName);
            }
        }

        private static void ReceiveDacServiceMessageEvent(object sender, DacMessageEventArgs e)
        {
            Console.WriteLine("Message Type:{0} Prefix:{1} Number:{2} Message:{3}", e.Message.MessageType,
                e.Message.Prefix, e.Message.Number, e.Message.Message);
        }

        private static void ReceiveDacServiceProgessEvent(object sender, DacProgressEventArgs e)
        {
            Console.WriteLine("Progress Event:{0} Progrss Status:{1}", e.Message, e.Status);
        }

        #endregion
    }
}