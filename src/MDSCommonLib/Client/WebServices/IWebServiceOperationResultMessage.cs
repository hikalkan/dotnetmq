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

namespace MDS.Client.WebServices
{
    /// <summary>
    /// Represents a result message for an incoming message to MDS Web Service.
    /// </summary>
    public interface IWebServiceOperationResultMessage
    {
        /// <summary>
        /// Operation result.
        /// True, if operation is successful.
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// A text that may be used as a description for result of operation.
        /// </summary>
        string ResultText { get; set; }
    }
}
