using System;
using System.Net;

namespace BacpacExtractor
{
    internal class Program
    {
        private const string RemoteUri = "http://eduapp.edumod2.net/Subscripciones/DatosColegios/DescargarBackup?nombreArchivo=";
        private const string FileName = "EX-WUS-Educamos_100095-20150512021703.bacpac";

        private static void Main(string[] args)
        {
            DownloadFile();
        }

        private static void DownloadFile()
        {
            string myStringWebResource = null;
            // Create a new WebClient instance.
            var myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            myStringWebResource = RemoteUri + FileName;
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", FileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, FileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", FileName, myStringWebResource);
        }
    }
}