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

using MDS.Exceptions;

namespace MDS.Communication.Messages
{
    public static class MDSMessageFactory
    {
        public const int MessageTypeIdMDSDataTransferMessage = 1;
        public const int MessageTypeIdMDSOperationResultMessage = 2;
        public const int MessageTypeIdMDSPingMessage = 3;
        public const int MessageTypeIdMDSRegisterMessage = 4;
        public const int MessageTypeIdMDSChangeCommunicationWayMessage = 5;
        public const int MessageTypeIdMDSControllerMessage = 6;
        public const int MessageTypeIdMDSDataTransferResponseMessage = 7;

        public static MDSMessage CreateMessageByTypeId(int messageTypeId)
        {
            switch (messageTypeId)
            {
                case MessageTypeIdMDSDataTransferMessage:
                    return new MDSDataTransferMessage();
                case MessageTypeIdMDSOperationResultMessage:
                    return new MDSOperationResultMessage();
                case MessageTypeIdMDSPingMessage:
                    return new MDSPingMessage();
                case MessageTypeIdMDSRegisterMessage:
                    return new MDSRegisterMessage();
                case MessageTypeIdMDSChangeCommunicationWayMessage:
                    return new MDSChangeCommunicationWayMessage();
                case MessageTypeIdMDSControllerMessage:
                    return new MDSControllerMessage();
                case MessageTypeIdMDSDataTransferResponseMessage:
                    return new MDSDataTransferResponseMessage();
                default:
                    throw new MDSException("Unknown MessageTypeId: " + messageTypeId);
            }
        }
    }
}
