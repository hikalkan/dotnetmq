/*
DotNetMQ - A Complete Message Broker For .NET
Copyright (C) 2011 Halil ibrahim KALKAN

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System.Collections.Generic;
using System.IO;
using System.Xml;
using MDS.Exceptions;

namespace MDS.Settings
{
    /// <summary>
    /// This class is used to get/set all design settings for this server from/to an XML file.
    /// Design settings is used by MDS Manager GUI.
    /// </summary>
    public class MDSDesignSettings
    {
        #region Static properties

        /// <summary>
        /// Singleton instance of MDSSettings.
        /// </summary>
        public static MDSDesignSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_synObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new MDSDesignSettings(Path.Combine(GeneralHelper.GetCurrentDirectory(), "MDSSettings.design.xml"));
                        }
                    }
                }

                return _instance;
            }

            set
            {
                lock (_synObj)
                {
                    _instance = value;
                }
            }
        }

        private static MDSDesignSettings _instance;

        private static readonly object _synObj = new object();

        #endregion

        #region Public properties

        /// <summary>
        /// All defined MDS servers in server graph.
        /// </summary>
        public List<ServerDesignItem> Servers { get; private set; }

        /// <summary>
        /// Path of XML design settings file.
        /// </summary>
        public string FilePath { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new MDSSettings from XML file.
        /// </summary>
        /// <param name="settingsFilePath">Path of xml file.</param>
        public MDSDesignSettings(string settingsFilePath)
        {
            FilePath = settingsFilePath;
            Servers = new List<ServerDesignItem>();
            LoadFromXml();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves current design settings to XML file.
        /// </summary>
        public void SaveToXml()
        {
            //Create directory if needed
            var saveDirectory = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            //Create XmlDocument object to create XML file
            var xmlDoc = new XmlDocument();

            //XML declaration
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);

            //Root node
            var rootNode = xmlDoc.CreateElement("MDSConfiguration");
            xmlDoc.AppendChild(rootNode);

            //Servers node
            var serversRootNode = xmlDoc.CreateElement("Servers");
            rootNode.AppendChild(serversRootNode);

            //Server nodes
            foreach (var server in Servers)
            {
                var serverNode = xmlDoc.CreateElement("Server");
                serverNode.SetAttribute("Name", server.Name);
                serverNode.SetAttribute("Location", server.Location);
                serversRootNode.AppendChild(serverNode);
            }

            //Save XML document
            xmlDoc.Save(FilePath);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets all settings from XML file.
        /// </summary>
        private void LoadFromXml()
        {
            //Create XmlDocument object to read XML settings file
            var settingsXmlDoc = new XmlDocument();
            settingsXmlDoc.Load(FilePath);

            //Get Servers section
            var resultNodes = settingsXmlDoc.SelectNodes("/MDSConfiguration/Servers/Server");
            if (resultNodes == null)
            {
                throw new MDSException("No server defined");
            }

            foreach (XmlNode node in resultNodes)
            {
                Servers.Add(new ServerDesignItem
                                {
                                    Name = node.Attributes["Name"].Value.Trim(),
                                    Location = node.Attributes["Location"].Value
                                });
            }
        }

        #endregion
    }
}
