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
using System.Data;
using MDS.Communication.Messages;
using MDS.Exceptions;
using MDS.Serialization;
using System.Data.SqlClient;

namespace MDS.Storage.MsSqlStorage
{
    /// <summary>
    /// This class is used to perform database operations on MS SQL Server database engine.
    /// </summary>
    public class MsSqlStorageManager : IStorageManager
    {
        #region Private fields

        /// <summary>
        /// Connection string to connect database.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
        private string _connectionString = "Data Source=localhost;Initial Catalog=mds;Integrated Security=SSPI;";

        #endregion

        #region Public methods

        #region Unimplemented methods

        public void Start()
        {
            //No action
        }

        public void Stop(bool waitToStop)
        {
            //No action
        }

        public void WaitToStop()
        {
            //No action
        }

        #endregion

        /// <summary>
        /// Saves a MDSMessageRecord.
        /// </summary>
        /// <param name="messageRecord">MDSMessageRecord object to save</param>
        /// <returns>Auto Increment Id of saved message</returns>
        public int StoreMessage(MDSMessageRecord messageRecord)
        {
            var bytesOfMessage = MDSSerializationHelper.SerializeToByteArray(messageRecord.Message);
            var id = InsertAndGetLastId(
                "INSERT INTO Messages(MessageId, DestServer, NextServer, DestApplication, MessageData, MessageDataLength, RecordDate) VALUES(@MessageId, @DestServer, @NextServer, @DestApplication, @MessageData, @MessageDataLength, GetDate())",
                new SqlParameter("@MessageId", messageRecord.MessageId),
                new SqlParameter("@DestServer", messageRecord.DestServer),
                new SqlParameter("@NextServer", messageRecord.NextServer),
                new SqlParameter("@DestApplication", messageRecord.DestApplication),
                new SqlParameter("@MessageData", bytesOfMessage),
                new SqlParameter("@MessageDataLength", bytesOfMessage.Length)
                );
            messageRecord.Id = id;
            return id;

        }

        /// <summary>
        /// Gets waiting messages for an application.
        /// </summary>
        /// <param name="nextServer">Next server name</param>
        /// <param name="destApplication">Destination application name</param>
        /// <param name="minId">Minimum Id (as start Id)</param>
        /// <param name="maxCount">Max record count to get</param>
        /// <returns>Records gotten from database.</returns>
        public List<MDSMessageRecord> GetWaitingMessagesOfApplication(string nextServer, string destApplication, int minId, int maxCount)
        {
            var recordsTable = GetTable(
                "SELECT TOP " + maxCount + " * FROM Messages WHERE NextServer = @NextServer AND DestApplication = @DestApplication AND Id >= @Id ORDER BY Id ASC",
                new SqlParameter("@NextServer", nextServer),
                new SqlParameter("@DestApplication", destApplication),
                new SqlParameter("@Id", minId)
                );

            var recordsList = new List<MDSMessageRecord>(recordsTable.Rows.Count);
            foreach (DataRow recordRow in recordsTable.Rows)
            {
                recordsList.Add(
                    new MDSMessageRecord
                        {
                            Id = Convert.ToInt32(recordRow["Id"]),
                            DestApplication = recordRow["DestApplication"] as string,
                            DestServer = recordRow["DestServer"] as string,
                            NextServer = recordRow["NextServer"] as string,
                            Message = MDSSerializationHelper.DeserializeFromByteArray(() => new MDSDataTransferMessage(), (byte[]) recordRow["MessageData"]),
                            MessageId = recordRow["MessageId"] as string,
                            RecordDate = (DateTime) recordRow["RecordDate"]
                        });
            }

            return recordsList;
        }

        /// <summary>
        /// Gets last (biggest) Id of waiting messages for an application.
        /// </summary>
        /// <param name="nextServer">Next server name</param>
        /// <param name="destApplication">Destination application name</param>
        /// <returns>last (biggest) Id of waiting messages</returns>
        public int GetMaxWaitingMessageIdOfApplication(string nextServer, string destApplication)
        {
            return GetScalarField(
                "SELECT TOP 1 Id FROM Messages WHERE NextServer = @NextServer AND DestApplication = @DestApplication ORDER BY Id DESC",
                new SqlParameter("@NextServer", nextServer),
                new SqlParameter("@DestApplication", destApplication)
                );
        }

        /// <summary>
        /// Gets waiting messages for an MDS server.
        /// </summary>
        /// <param name="nextServer">Next server name</param>
        /// <param name="minId">Minimum Id (as start Id)</param>
        /// <param name="maxCount">Max record count to get</param>
        /// <returns>Records gotten from database.</returns>
        public List<MDSMessageRecord> GetWaitingMessagesOfServer(string nextServer, int minId, int maxCount)
        {
            var recordsTable = GetTable(
                "SELECT TOP " + maxCount + " * FROM Messages WHERE NextServer = @NextServer AND Id >= @Id ORDER BY Id ASC",
                new SqlParameter("@NextServer", nextServer),
                new SqlParameter("@Id", minId)
                );

            var recordsList = new List<MDSMessageRecord>(recordsTable.Rows.Count);
            foreach (DataRow recordRow in recordsTable.Rows)
            {
                recordsList.Add(
                    new MDSMessageRecord
                        {
                            Id = Convert.ToInt32(recordRow["Id"]),
                            DestApplication = recordRow["DestApplication"] as string,
                            DestServer = recordRow["DestServer"] as string,
                            NextServer = recordRow["NextServer"] as string,
                            Message = MDSSerializationHelper.DeserializeFromByteArray(() => new MDSDataTransferMessage(), (byte[]) recordRow["MessageData"]),
                            MessageId = recordRow["MessageId"] as string,
                            RecordDate = (DateTime) recordRow["RecordDate"]
                        });
            }

            return recordsList;
        }

        /// <summary>
        /// Gets last (biggest) Id of waiting messages for an MDS server.
        /// </summary>
        /// <param name="nextServer">Next server name</param>
        /// <returns>last (biggest) Id of waiting messages</returns>
        public int GetMaxWaitingMessageIdOfServer(string nextServer)
        {
            return GetScalarField(
                "SELECT TOP 1 Id FROM Messages WHERE NextServer = @NextServer ORDER BY Id DESC",
                new SqlParameter("@NextServer", nextServer)
                );
        }

        /// <summary>
        /// Removes a message.
        /// </summary>
        /// <param name="id">id of message to remove</param>
        /// <returns>Effected rows count</returns>
        public int RemoveMessage(int id)
        {
            return ExecuteNonQuery(
                "DELETE FROM Messages WHERE Id = @Id",
                new SqlParameter("@Id", id)
                );
        }

        /// <summary>
        /// This method is used to set Next Server for a Destination Server. 
        /// It is used to update database records when Server Graph changed.
        /// </summary>
        /// <param name="destServer">Destination server of messages</param>
        /// <param name="nextServer">Next server of messages for destServer</param>
        public void UpdateNextServer(string destServer, string nextServer)
        {
            ExecuteNonQuery(
                "UPDATE Messages SET NextServer = @NextServer WHERE DestServer = @DestServer",
                new SqlParameter("@NextServer", nextServer),
                new SqlParameter("@DestServer", destServer)
                );
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Executes a query and returns effected rows count.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Effected rows count</returns>
        private int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// This method is used to run an insert query and get inserted row's auto increment column's value.
        /// </summary>
        /// <param name="query">Insert query to be executed</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Auto increment column's value of inserted row</returns>
        private int InsertAndGetLastId(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    command.ExecuteNonQuery();
                }

                const string queryForLastId = "SELECT @@Identity AS LastId;";
                using (var command = new SqlCommand(queryForLastId, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["LastId"]);
                        }

                        throw new MDSException("Can not be obtained last inserted id for query: " + query);
                    }
                }
            }
        }

        /// <summary>
        /// Runs a query and returns a DataTable.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Selected table</returns>
        private DataTable GetTable(string query, params SqlParameter[] parameters)
        {
            var table = new DataTable();
            using (var connection = new SqlConnection(ConnectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a record from a table.
        /// </summary>
        /// <param name="query">Select query</param>
        /// <param name="parameters">Select parameters</param>
        /// <returns>Returns found row as TableRecord object. If there is no row returns null</returns>
        public TableRecord GetTableRecord(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var record = new TableRecord();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                record[reader.GetName(i)] = reader[i];
                            }

                            return record;
                        }

                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Executes a query and gets a Integer result.
        /// If query returns no data, method returns 0.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Query result or 0</returns>
        public int GetScalarField(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader[0]);
                        }

                        return 0;
                    }
                }
            }
        }

        #endregion
    }
}
