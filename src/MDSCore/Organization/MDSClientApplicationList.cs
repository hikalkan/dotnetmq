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

using System;
using System.Collections.Generic;
using MDS.Communication.WebServiceCommunication;
using MDS.Exceptions;
using MDS.Settings;
using MDS.Threading;

namespace MDS.Organization
{
    /// <summary>
    /// All Client applications that can send/receive messages to/from this MDS server are stored in this class.
    /// </summary>
    public class MDSClientApplicationList : IRunnable
    {
        #region Public properties

        /// <summary>
        /// A collection that stores client applications.
        /// MDSClientApplication objects count is equals to total application definition in Settings file.
        /// </summary>
        public SortedList<string, MDSClientApplication> Applications { get; private set; }

        /// <summary>
        /// Reference to settings.
        /// </summary>
        private readonly MDSSettings _settings;

        #endregion

        #region Constrcutors

        /// <summary>
        /// Contructor. Gets applications from given settings file.
        /// </summary>
        public MDSClientApplicationList()
        {
            _settings = MDSSettings.Instance;
            Applications = new SortedList<string, MDSClientApplication>();
            try
            {
                CreateApplicationList();
            }
            catch (MDSException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MDSException("Can not read settings file.", ex);
            }
        }

        #endregion

        #region Public methods

        public void Start()
        {
            foreach (var application in Applications.Values)
            {
                application.Start();
            }
        }

        public void Stop(bool waitToStop)
        {
            foreach (var application in Applications.Values)
            {
                application.Stop(waitToStop);
            }
        }

        public void WaitToStop()
        {
            foreach (var application in Applications.Values)
            {
                application.WaitToStop();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Reads the xml file and creates client applications using _settings.
        /// </summary>
        private void CreateApplicationList()
        {
            foreach (var application in _settings.Applications)
            {
                //Create application object
                var clientApplication = new MDSClientApplication(application.Name);

                foreach (var channel in application.CommunicationChannels)
                {
                    switch (channel.CommunicationType)
                    {
                        case "WebService":
                            clientApplication.AddCommunicator(
                                new WebServiceCommunicator(
                                    channel.CommunicationSettings["Url"],
                                    Communication.CommunicationLayer.CreateCommunicatorId()
                                    ));
                            break;
                    }
                }

                //Add new application to list
                Applications.Add(clientApplication.Name, clientApplication);
            }
        }

        #endregion
    }
}
