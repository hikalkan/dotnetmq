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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using MDS.Exceptions;

namespace MDS
{
    /// <summary>
    /// This class is used to perform common tasks that is used in both client and server side.
    /// </summary>
    public static class GeneralHelper
    {
        /// <summary>
        /// This code is used to connect to a TCP socket with timeout option.
        /// </summary>
        /// <param name="endPoint">IP endpoint of remote server</param>
        /// <param name="timeoutMs">Timeout to wait until connect</param>
        /// <returns>Socket object connected to server</returns>
        public static Socket ConnectToServerWithTimeout(EndPoint endPoint, int timeoutMs)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Blocking = false;
                socket.Connect(endPoint);
                socket.Blocking = true;
                return socket;
            }
            catch (SocketException socketException)
            {
                if (socketException.ErrorCode != 10035)
                {
                    socket.Close();
                    throw;
                }

                if (!socket.Poll(timeoutMs * 1000, SelectMode.SelectWrite))
                {
                    socket.Close();
                    throw new MDSException("The host failed to connect. Timeout occured.");
                }

                socket.Blocking = true;
                return socket;
            }
        }

        public static byte[] SerializeObject(object obj)
        {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }

        public static object DeserializeObject(byte[] bytesOfObject)
        {
            return new BinaryFormatter().Deserialize(new MemoryStream(bytesOfObject) { Position = 0 });
        }

        /// <summary>
        /// Gets the current directory of executing assembly.
        /// </summary>
        /// <returns>Directory path</returns>
        public static string GetCurrentDirectory()
        {
            string directory;
            try
            {
                directory = (new FileInfo(Assembly.GetExecutingAssembly().Location)).Directory.FullName;
            }
            catch (Exception)
            {
                directory = Directory.GetCurrentDirectory();
            }

            var directorySeparatorChar = Path.DirectorySeparatorChar.ToString();
            if (!directory.EndsWith(directorySeparatorChar))
            {
                directory += directorySeparatorChar;
            }

            return directory;
        }
    }
}
