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
using MDS.Communication.Messages;

namespace MDS.Storage
{
    /// <summary>
    /// Represents a message record in database/storage manager.
    /// </summary>
    public class MDSMessageRecord
    {
        /// <summary>
        /// Auto Increment ID in database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// MessageId of message.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Message object.
        /// </summary>
        public MDSDataTransferMessage Message { get; set; }

        /// <summary>
        /// Destination server.
        /// </summary>
        public string DestServer { get; set; }

        /// <summary>
        /// Next server.
        /// </summary>
        public string NextServer { get; set; }

        /// <summary>
        /// Destination application in destination server
        /// </summary>
        public string DestApplication { get; set; }

        /// <summary>
        /// Storing time of message on this server.
        /// </summary>
        public DateTime RecordDate { get; set; }

        /// <summary>
        /// Empty contructor.
        /// </summary>
        public MDSMessageRecord()
        {
            
        }

        /// <summary>
        /// Creates a MDSMessageRecord object using a MDSDataTransferMessage.
        /// </summary>
        /// <param name="message">Message object</param>
        public MDSMessageRecord(MDSDataTransferMessage message)
        {
            Message = message;
            MessageId = message.MessageId;
            DestServer = message.DestinationServerName;
            DestApplication = message.DestinationApplicationName;
            RecordDate = DateTime.Now;
        }
    }
}
