﻿//--------------------------------------------------//
// Created by basicx-StrgV                          //
// https://github.com/basicx-StrgV/                 //
//--------------------------------------------------//
using System;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

namespace WGetNET
{
    /// <summary>
    /// The <see cref="WGetNET.WinGetSourceManager"/> class offers methods to manage the sources used by winget.
    /// </summary>
    public class WinGetSourceManager
    {
        private const string _sourceListCmd = "source list";
        private const string _sourceAddCmd = "source add -n {0} -a {1} --accept-source-agreements";
        private const string _sourceAddWithTypeCmd = "source add -n {0} -a {1} -t {2} --accept-source-agreements";
        private const string _sourceUpdateCmd = "source update";
        private const string _sourceExportCmd = "source export";

        private readonly ProcessManager _processManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WGetNET.WinGetSourceManager"/> class.
        /// </summary>
        public WinGetSourceManager()
        {
            _processManager = new ProcessManager();
        }

        //---List--------------------------------------------------------------------------------------
        /// <summary>
        /// Gets a list of all sources that are installed in winget.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{T}"/> of <see cref="WGetNET.WinGetSource"/> instances.
        /// </returns>
        public List<WinGetSource> GetInstalledSources()
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceListCmd);

                return ToSourceList(result.Output);
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------

        //---Add---------------------------------------------------------------------------------------
        /// <summary>
        /// Adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source to add.
        /// </param>
        /// <param name="arg">
        /// A <see cref="System.String"/> representing the source (eg. URL).
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        public bool AddSource(string name, string arg)
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(
                        String.Format(_sourceAddCmd, name, arg));

                if (result.ExitCode == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }

        /// <summary>
        /// Adds a new source to winget (Needs administrator rights).
        /// </summary>
        /// <remarks>
        /// The source type is optional but some sources like the "msstore" need it or adding it wil throw an error.
        /// </remarks>
        /// <param name="name">
        /// A <see cref="System.String"/> representing the name of the source to add.
        /// </param>
        /// <param name="arg">
        /// A <see cref="System.String"/> representing the source (eg. URL).
        /// </param>
        /// <param name="type">
        /// A <see cref="System.String"/> representing the source type.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the action was succesfull and <see langword="false"/> if it failed.
        /// </returns>
        public bool AddSource(string name, string arg, string type)
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(
                        String.Format(_sourceAddWithTypeCmd, name, arg, type));

                if (result.ExitCode == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Getting installed sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------

        //---Update------------------------------------------------------------------------------------
        /// <summary>
        /// Updates all sources that are installed in winget.
        /// </summary>
        /// <remarks>
        /// This may take a while depending on the sources.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the update was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        public bool UpdateSources()
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceUpdateCmd);

                //Check if installation was succsessfull
                if (result.ExitCode == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Updating sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------
        
        //---Export------------------------------------------------------------------------------------
        /// <summary>
        /// Exports the winget sources as a json string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        public string ExportSources()
        {
            try
            {
                ProcessResult result = 
                    _processManager.ExecuteWingetProcess(_sourceExportCmd);

                StringBuilder outputBuilder = new StringBuilder();
                foreach (string line in result.Output)
                {
                    outputBuilder.Append(line);
                }

                if (result.ExitCode == 0)
                {
                    return outputBuilder.ToString().Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources as a json string.
        /// </summary>
        /// <param name="sourceName">The name of the source for the export.</param>
        /// <returns>
        /// A <see cref="System.String"/> that contains the winget sorces in json format.
        /// </returns>
        public string ExportSources(string sourceName)
        {
            try
            {
                //Set Arguments
                string cmd =
                    _sourceExportCmd +
                    " -n " +
                    sourceName;

                ProcessResult result =
                    _processManager.ExecuteWingetProcess(cmd);

                StringBuilder outputBuilder = new StringBuilder();
                foreach (string line in result.Output)
                {
                    outputBuilder.Append(line);
                }

                if (result.ExitCode == 0)
                {
                    return outputBuilder.ToString().Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">The file for the export.</param>
        /// <returns>
        /// <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        public bool ExportSourcesToFile(string file)
        {
            try
            {
                ProcessResult result =
                    _processManager.ExecuteWingetProcess(_sourceExportCmd);

                StringBuilder outputBuilder = new StringBuilder();
                foreach (string line in result.Output)
                {
                    outputBuilder.Append(line);
                }

                if (result.ExitCode == 0 && outputBuilder.ToString().Trim() != string.Empty)
                {
                    File.WriteAllText(
                        file, 
                        outputBuilder
                        .ToString()
                        .Trim());
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }

        /// <summary>
        /// Exports the winget sources in json format to a file.
        /// </summary>
        /// <param name="file">The file for the export.</param>
        /// <param name="sourceName">The name of the source for the export.</param>
        /// <returns>
        /// <see langword="true"/> if the export was successfull or <see langword="false"/> if the it failed.
        /// </returns>
        public bool ExportSourcesToFile(string file, string sourceName)
        {
            try
            {
                //Set Arguments
                string cmd =
                    _sourceExportCmd +
                    " -n " +
                    sourceName;

                ProcessResult result =
                    _processManager.ExecuteWingetProcess(cmd);

                StringBuilder outputBuilder = new StringBuilder();
                foreach (string line in result.Output)
                {
                    outputBuilder.Append(line);
                }

                if (result.ExitCode == 0 && outputBuilder.ToString().Trim() != string.Empty)
                {
                    File.WriteAllText(
                        file,
                        outputBuilder
                        .ToString()
                        .Trim());
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Win32Exception)
            {
                throw new WinGetNotInstalledException();
            }
            catch (Exception e)
            {
                throw new WinGetActionFailedException("Exporting sources failed.", e);
            }
        }
        //---------------------------------------------------------------------------------------------

        private List<WinGetSource> ToSourceList(List<string> output)
        {
            //Get top line index
            int topLineIndex = 0;
            for (int i = 0; i < output.Count; i++)
            {
                if (output[i].Contains("------"))
                {
                    topLineIndex = i;
                    break;
                }
            }

            //Get start indexes of each tabel colum
            int nameStartIndex = 0;

            int urlStartIndex = 0;

            int labelLine = topLineIndex - 1;
            bool checkForChar = false;
            for (int i = 0; i < output[labelLine].Length; i++)
            {
                if (output[labelLine][i] != ' ' && checkForChar)
                {
                    urlStartIndex = i;
                    break;
                }
                else if (output[labelLine][i] == ' ')
                {
                    checkForChar = true;
                }
            }

            //Remove unneeded output Lines
            output.RemoveRange(0, topLineIndex + 1);

            List<WinGetSource> resultList = new List<WinGetSource>();

            foreach (string line in output)
            {
                string name = 
                    line[nameStartIndex..urlStartIndex]
                    .Trim();
                string winGetId = 
                    line[urlStartIndex..]
                    .Trim();

                resultList.Add(
                    new WinGetSource() 
                    { 
                        SourceName = name, 
                        SourceUrl = winGetId 
                    });
            }

            return resultList;
        }
    }
}
