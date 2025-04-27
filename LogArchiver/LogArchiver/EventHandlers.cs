using System;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace LogArchiver;

public class EventHandlers
{
    public void OnWaitingForPlayers()
    {
        if (Plugin.Instance.Config.LogArchive)
        {
            if (Plugin.Instance.Config.filePath.IsEmpty())
            {
                Log.Warn($"The list of file paths is currently empty.");
                return;
            }
            foreach (string filePath in Plugin.Instance.Config.filePath)
            {
                if (Directory.Exists(filePath))
                {
                    int numberFile = Directory.GetFiles(filePath, "*.txt").Length;
                    Log.Debug("Number of Txt file: " + numberFile);
                    if (numberFile > Plugin.Instance.Config.MaxLogNumber)
                    {
                        Log.Debug("Number of Txt file exceeded: " + numberFile);
                        CreateTarGzFromTxtFiles(filePath, filePath + $"/{DateTime.Now:yyyyMMdd_HHmmss}.tar.gz");
                    }
            
                    int TarFile = Directory.GetFiles(filePath, "*.tar.gz").Length;
                    Log.Debug("Number of Tar file: " + TarFile);
                    if (TarFile > Plugin.Instance.Config.MaxArchiveLogNumber)
                    {
                        Log.Debug("Number of Tar file exceeded: " + TarFile);
                        FindOldestTarGz(filePath);
                    }
                }
                else
                {
                    Log.Warn($"The path doesn't exist or are empty: {Plugin.Instance.Config.filePath}");
                    return;
                }
            }
        }

        if (Plugin.Instance.Config.LogRaArchive)
        {
            string pathRA = Plugin.Instance.Config.LogRaDirectory;
            if (!Directory.Exists(pathRA))
            {
                Log.Warn($"The path for RA Log doesn't exist or are empty: {pathRA}");
                return;
            }
            string[] RALogFile = Directory.GetFiles(pathRA, "*.txt");
            foreach (string file in RALogFile)
            {
                long sizeFile = new FileInfo(file).Length;
                Log.Debug($"RA file size: {sizeFile}");
                if (sizeFile > Plugin.Instance.Config.MaxRaLogSize)
                {
                    Log.Debug("RA file exceeded: " + sizeFile);
                    CreateTarGzFromTxtFiles(pathRA, pathRA+ $"/{DateTime.Now:yyyyMMdd_HHmmss}.tar.gz");
                }
            }
            
            int TarFile = Directory.GetFiles(pathRA, "*.tar.gz").Length;
            Log.Debug("Number of Tar file: " + TarFile);
            if (TarFile > Plugin.Instance.Config.MaxRaLogArchive)
            {
                Log.Debug("Number of Tar file exceeded: " + TarFile);
                FindOldestTarGz(pathRA);
            }
        }
    }
    static void FindOldestTarGz(string filePath)
    {
        string[] TarFile = Directory.GetFiles(filePath, "*.tar.gz");
        string OldestFile = TarFile.Select(f => new FileInfo(f))
            .OrderBy(f => f.CreationTimeUtc)
            .First()
            .FullName;
        Log.Debug($"File to delete: {OldestFile}");
        File.Delete(OldestFile);
    }
    static void CreateTarGzFromTxtFiles(string sourceDirectory, string DestinationArchive)
    {
        using (FileStream outStream = File.Create(DestinationArchive))
        using (GZipOutputStream gzipStream = new GZipOutputStream(outStream))
        using (TarOutputStream tarStream = new TarOutputStream(gzipStream, System.Text.Encoding.ASCII))
        {
            tarStream.IsStreamOwner = false;

            string[] fichiersTxt = Directory.GetFiles(sourceDirectory, "*.txt");

            foreach (string fichier in fichiersTxt)
            {
                AddFileToTar(tarStream, fichier);
                File.Delete(fichier);
            }

            tarStream.Close();
        }
        Log.Debug($"Successfully created {DestinationArchive}");
    }
    static void AddFileToTar(TarOutputStream tarStream, string filePath)
    {
        string nameFile = Path.GetFileName(filePath);
        TarEntry entry = TarEntry.CreateEntryFromFile(filePath);
        entry.Name = nameFile;
        tarStream.PutNextEntry(entry);

        using (FileStream fichierStream = File.OpenRead(filePath))
        {
            fichierStream.CopyTo(tarStream);
        }
        tarStream.CloseEntry();
    }
}