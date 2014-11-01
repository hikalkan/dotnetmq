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

namespace MDS.Client.AppService
{
    /// <summary>
    /// Represents a MDS Application from MDSMessageProcessor perspective.
    /// This class also provides a static collection that can be used as a cache,
    /// thus, an MDSMessageProcessor/MDSClientApplicationBase can store/get application-wide objects.
    /// </summary>
    public interface IMDSApplication
    {
        /// <summary>
        /// Name of the application.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// Gets/Sets application-wide object by a string key.
        /// </summary>
        /// <param name="key">Key of the object to access it</param>
        /// <returns>Object, that is related with given key</returns>
        object this[string key] { get; set; }
    }
}
