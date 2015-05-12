using System;
using System.Net;

namespace BacpacExtractor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string remoteUri =
                "http://eduapp.edumod2.net/Subscripciones/DatosColegios/DescargarBackup?nombreArchivo=";
            const string fileName = "EX-WUS-Educamos_100095-20150512021703.bacpac";
            string myStringWebResource = null;
            // Create a new WebClient instance.
            var myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri + fileName;
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
        }
    }
}