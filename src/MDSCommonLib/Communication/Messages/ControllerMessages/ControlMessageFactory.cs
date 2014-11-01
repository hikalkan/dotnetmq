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

namespace MDS.Communication.Messages.ControllerMessages
{
    public static class ControlMessageFactory
    {
        public const int MessageTypeIdGetApplicationListMessage = 1;
        public const int MessageTypeIdGetApplicationListResponseMessage = 2;
        public const int MessageTypeIdClientApplicationRefreshEventMessage = 3;
        public const int MessageTypeIdAddNewApplicationMessage = 4;
        public const int MessageTypeIdRemoveApplicationMessage = 5;
        public const int MessageTypeIdRemoveApplicationResponseMessage = 6;
        public const int MessageTypeIdClientApplicationRemovedEventMessage = 7;
        public const int MessageTypeIdGetServerGraphMessage = 8;
        public const int MessageTypeIdGetServerGraphResponseMessage = 9;
        public const int MessageTypeIdUpdateServerGraphMessage = 10;
        public const int MessageTypeIdOperationResultMessage = 11;
        public const int MessageTypeIdGetApplicationWebServicesMessage = 12;
        public const int MessageTypeIdGetApplicationWebServicesResponseMessage = 13;
        public const int MessageTypeIdUpdateApplicationWebServicesMessage = 14;

        public static ControlMessage CreateMessageByTypeId(int messageTypeId)
        {
            switch (messageTypeId)
            {
                case MessageTypeIdGetApplicationListMessage:
                    return new GetApplicationListMessage();
                case MessageTypeIdGetApplicationListResponseMessage:
                    return new GetApplicationListResponseMessage();
                case MessageTypeIdClientApplicationRefreshEventMessage:
                    return new ClientApplicationRefreshEventMessage();
                case MessageTypeIdAddNewApplicationMessage:
                    return new AddNewApplicationMessage();
                case MessageTypeIdRemoveApplicationMessage:
                    return new RemoveApplicationMessage();
                case MessageTypeIdRemoveApplicationResponseMessage:
                    return new RemoveApplicationResponseMessage();
                case MessageTypeIdClientApplicationRemovedEventMessage:
                    return new ClientApplicationRemovedEventMessage();
                case MessageTypeIdGetServerGraphMessage:
                    return new GetServerGraphMessage();
                case MessageTypeIdGetServerGraphResponseMessage:
                    return new GetServerGraphResponseMessage();
                case MessageTypeIdUpdateServerGraphMessage:
                    return new UpdateServerGraphMessage();
                case MessageTypeIdOperationResultMessage:
                    return new OperationResultMessage();
                case MessageTypeIdGetApplicationWebServicesMessage:
                    return new GetApplicationWebServicesMessage();
                case MessageTypeIdGetApplicationWebServicesResponseMessage:
                    return new GetApplicationWebServicesResponseMessage();
                case MessageTypeIdUpdateApplicationWebServicesMessage:
                    return new UpdateApplicationWebServicesMessage();
                default:
                    throw new MDSException("Undefined ControlMessage MessageTypeId: " + messageTypeId);
            }
        }
    }
}
