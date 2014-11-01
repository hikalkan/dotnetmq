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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using log4net;
using MDS.Communication;
using MDS.Communication.Messages.ControllerMessages;
using MDS.Exceptions;
using MDS.GUI;

namespace MDS.Management.GUI.MDSServers
{
    /// <summary>
    /// This form is used to design Server graph of MDS.
    /// </summary>
    public partial class ServerGraphForm : Form
    {
        #region Private properties

        /// <summary>
        /// Reference to logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Reference to MDSController object.
        /// All communication with MDS server is performed using this object.
        /// </summary>
        private readonly MDSController _controller;

        /// <summary>
        /// Reference to this MDS server on graph.
        /// </summary>
        private MDSServer _thisServer;

        /// <summary>
        /// All MDS servers on graph.
        /// </summary>
        private readonly List<MDSServer> _servers;

        /// <summary>
        /// Stores all graph information that are gotten from MDS server.
        /// </summary>
        private ServerGraphInfo _serverGraphInfo;

        #region Designing fields

        /// <summary>
        /// This is the first clicked label to create a link between servers
        /// </summary>
        private Label _selectedLabel;

        /// <summary>
        /// X coordinate of mouse in selected label.
        /// </summary>
        private int _movingLabelOffsetX;

        /// <summary>
        /// Y coordinate of mouse in selected label.
        /// </summary>
        private int _movingLabelOffsetY;

        /// <summary>
        /// X coordinate of label that is guiding for replacement to _movingLabel.
        /// </summary>
        private int _movingLabelGuideLabelX = -1;

        /// <summary>
        /// Y coordinate of label that is guiding for replacement to _movingLabel.
        /// </summary>
        private int _movingLabelGuideLabelY = -1;

        /// <summary>
        /// X coordinate of moouse at the last click in the design area.
        /// It is used to find initial location when adding new server.
        /// </summary>
        private int _designPanelLastX = -1;

        /// <summary>
        /// Y coordinate of moouse at the last click in the design area.
        /// It is used to find initial location when adding new server.
        /// </summary>
        private int _designPanelLastY = -1;

        #endregion

        #region Drawing fields

        /// <summary>
        /// This pen is used to draw lines between servers.
        /// </summary>
        private readonly Pen _linePen;

        /// <summary>
        /// This pen is used to draw lines when moving a label for guide lines.
        /// </summary>
        private readonly Pen _guideLinePen;

        /// <summary>
        /// This pen is used to select (to draw a outer rectangle to) a server label.
        /// </summary>
        private readonly Pen _labelSelectingPen;

        /// <summary>
        /// Background color of label of this server on graph.
        /// </summary>
        private readonly Color _thisServerBackColor;

        /// <summary>
        /// Background color of label of all servers on graph except this server.
        /// </summary>
        private readonly Color _serverBackColor;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ServerGraphForm.
        /// </summary>
        /// <param name="controller">Reference to MDSController object</param>
        public ServerGraphForm(MDSController controller)
        {
            _controller = controller;
            _servers = new List<MDSServer>();
            _linePen = new Pen(Brushes.Black, 1.0f);
            _guideLinePen = new Pen(Color.LightGray, 1.0f);
            _labelSelectingPen = new Pen(Color.Black, 1.0f) { DashStyle = DashStyle.DashDot };
            _thisServerBackColor = Color.Crimson;
            _serverBackColor = Color.DodgerBlue;
            InitializeComponent();
        }

        #endregion

        #region Form events

        private void ServerGraphForm_Load(object sender, EventArgs e)
        {
            try
            {
                PrepareForm();
                GetGraphFromServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not load graph. " + ex.Message);
                Close();
            }
        }
        
        private void btnSaveGraph_Click(object sender, EventArgs e)
        {
            btnSaveGraph.Enabled = false;
            Application.DoEvents();

            try
            {
                CheckGraph();
                _serverGraphInfo = CreateServerGraphInfo();
                
                //Send message to the server and get response
                var message = _controller.SendMessageAndGetResponse(
                    new UpdateServerGraphMessage {ServerGraph = _serverGraphInfo}
                    );

                //Check response message
                if (message.MessageTypeId != ControlMessageFactory.MessageTypeIdOperationResultMessage)
                {
                    throw new MDSException("Response message to UpdateServerGraphMessage must be a OperationResultMessage");
                }

                var updateResponseMessage = message as OperationResultMessage;
                if (updateResponseMessage == null)
                {
                    throw new MDSException("Incorrect message type. MessageTypeId = " + message.MessageTypeId + ", but Type of object: " + message.GetType().Name);
                }

                //Inform user about update result
                if (updateResponseMessage.Success)
                {
                    MDSGuiHelper.ShowInfoDialog("Server graph is successfully updated on server", "Success.");
                }
                else
                {
                    MDSGuiHelper.ShowErrorMessage(
                        "Server graph can not be updated on server. Reason: " + updateResponseMessage.ResultMessage,
                        "Failed!");
                }
            }
            catch (Exception ex)
            {
                MDSGuiHelper.ShowWarningMessage("Can not save graph. " + ex.Message);
            }
            finally
            {
                btnSaveGraph.Enabled = true;                
            }
        }

        private void ServerGraphForm_Resize(object sender, EventArgs e)
        {
            pnlDesign.Width = Width - 24;
            pnlDesign.Height = Height - 82;
            btnSaveGraph.Left = pnlDesign.Right - 200;
            btnSaveGraph.Top = pnlDesign.Bottom + 5;
        }

        private void ServerLabel_MouseDown(object sender, MouseEventArgs e)
        {
            var currentLabel = sender as Label;
            if (currentLabel == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (((ModifierKeys & Keys.Control) == Keys.Control) && (_selectedLabel != null))
                {
                    LinkOrUnlinkServers(_selectedLabel, currentLabel);
                }

                currentLabel.BackColor = Color.Black;
                _movingLabelOffsetX = e.X;
                _movingLabelOffsetY = e.Y;
            }

            _selectedLabel = currentLabel;
            pnlDesign.Invalidate();
        }

        private void ServerLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if ((_selectedLabel != null) && (e.Button == MouseButtons.Left))
            {
                //Find normal X and Y ccordinate of mouse
                var labelX = e.X + _selectedLabel.Left - _movingLabelOffsetX;
                var labelY = e.Y + _selectedLabel.Top - _movingLabelOffsetY;

                //If Alt key is not pressed and there is a label whose coordinates are (5 pixel) close to this label's, than
                //hold server label, else move label...
                _movingLabelGuideLabelX = ((ModifierKeys & Keys.Alt) == Keys.Alt) ? int.MinValue : FindGuideLabelX(labelX);
                _selectedLabel.Left = (_movingLabelGuideLabelX > int.MinValue) ? _movingLabelGuideLabelX : labelX;
                _movingLabelGuideLabelY = ((ModifierKeys & Keys.Alt) == Keys.Alt) ? int.MinValue : FindGuideLabelY(labelY);
                _selectedLabel.Top = (_movingLabelGuideLabelY > int.MinValue) ? _movingLabelGuideLabelY : labelY;

                pnlDesign.Invalidate();
            }
        }

        private void ServerLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_selectedLabel != null && e.Button == MouseButtons.Left)
            {
                _selectedLabel.BackColor = (_thisServer != null && _selectedLabel == _thisServer.LabelOfServer)
                                               ? _thisServerBackColor
                                               : _serverBackColor;
                _movingLabelGuideLabelX = int.MinValue;
                _movingLabelGuideLabelY = int.MinValue;
                pnlDesign.Invalidate();
            }
        }

        private void DesignPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _designPanelLastX = e.X;
            _designPanelLastY = e.Y;

            if (_selectedLabel == null)
            {
                return;
            }

            _selectedLabel = null;
            pnlDesign.Invalidate();
        }

        private void ServerLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ServerRightMenu_ServerProperties_Click(sender, e);
        }

        private void DesignPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawAllLines(e.Graphics);
        }
        
        private void ServerRightMenu_RemoveServer_Click(object sender, EventArgs e)
        {
            var removingServer = GetSelectedServer();
            if (removingServer == null)
            {
                return;
            }

            if (removingServer.ServerInfo.Name == _thisServer.ServerInfo.Name)
            {
                MDSGuiHelper.ShowWarningMessage("You can not remove this server from graph!", "Attention!");
                return;
            }

            var dialogResult = MDSGuiHelper.ShowQuestionDialog(
                "Are you sure to remove MDS Server " + removingServer.ServerInfo.Name + " (" +
                removingServer.ServerInfo.IpAddress + ") from graph.", "Confirm removing server!");
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            foreach (var adjacent in removingServer.Adjacents)
            {
                if (adjacent.Adjacents.Contains(removingServer))
                {
                    adjacent.Adjacents.Remove(removingServer);
                }
            }

            _servers.Remove(removingServer);
            pnlDesign.Controls.Remove(removingServer.LabelOfServer);
            _selectedLabel = null;

            pnlDesign.Invalidate();
        }
        
        private void ServerRightMenu_SetAsThisServer_Click(object sender, EventArgs e)
        {
            var newThisServer = GetSelectedServer();
            if (newThisServer == null || newThisServer == _thisServer)
            {
                return;
            }

            if (_thisServer != null)
            {
                _thisServer.LabelOfServer.BackColor = _serverBackColor;
            }

            _thisServer = newThisServer;
            _thisServer.LabelOfServer.BackColor = _thisServerBackColor;
        }

        private void ServerRightMenu_ServerProperties_Click(object sender, EventArgs e)
        {
            var editingServer = GetSelectedServer();
            if (editingServer == null)
            {
                return;
            }

            var editServerForm = new EditServerForm(editingServer.ServerInfo, false);
            editServerForm.ShowDialog();
            if (!editServerForm.Updated)
            {
                return;
            }

            editingServer.LabelOfServer.Text = editingServer.ServerInfo.Name + Environment.NewLine + editingServer.ServerInfo.IpAddress;
            pnlDesign.Invalidate();
        }
        
        private void DesignArea_AddNewServer_Click(object sender, EventArgs e)
        {
            var newServer = new MDSServer
                                    {
                                        ServerInfo =
                                            new ServerGraphInfo.ServerOnGraph
                                                {
                                                    Name = "",
                                                    IpAddress = "",
                                                    Port = CommunicationConsts.DefaultMDSPort,
                                                    Adjacents = "",
                                                    Location = _designPanelLastX + "," + _designPanelLastY,
                                                }
                                    };

            var addServerForm = new EditServerForm(newServer.ServerInfo, true);
            addServerForm.ShowDialog();
            if (!addServerForm.Updated)
            {
                return;
            }

            newServer.LabelOfServer = CreateServerLabel(newServer);
            newServer.LabelOfServer.Text = newServer.ServerInfo.Name + Environment.NewLine + newServer.ServerInfo.IpAddress;
            newServer.LabelOfServer.Left = _designPanelLastX;
            newServer.LabelOfServer.Top = _designPanelLastY;
            _servers.Add(newServer);
            pnlDesign.Invalidate();
        }

        #endregion

        #region Private methods

        private void PrepareForm()
        {
            Left = (Screen.GetWorkingArea(this).Width - Width) / 2;
            Top = (Screen.GetWorkingArea(this).Height - Height) / 2;
        }

        /// <summary>
        /// Gets server graph from server and creates graph on screen.
        /// </summary>
        private void GetGraphFromServer()
        {
            try
            {
                //Send a message to MDS server to get list of client applications, get response and fill data grid.
                var message = _controller.SendMessageAndGetResponse(new GetServerGraphMessage());
                if (message.MessageTypeId != ControlMessageFactory.MessageTypeIdGetServerGraphResponseMessage)
                {
                    throw new MDSException("Response message to GetServerGraphMessage must be a GetServerGraphResponseMessage");
                }

                var serverGraphResponseMessage = message as GetServerGraphResponseMessage;
                if (serverGraphResponseMessage == null)
                {
                    throw new MDSException("Incorrect message type. MessageTypeId = " + message.MessageTypeId + ", but Type of object: " + message.GetType().Name);
                }

                _serverGraphInfo = serverGraphResponseMessage.ServerGraph;
                CreateGraph();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MDSGuiHelper.ShowErrorMessage("Application list can not received from MDS Server. Detail: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates graph on design area from _serverGraphInfo.
        /// </summary>
        private void CreateGraph()
        {
            //Create MDSServerNode objects and get names of adjacents of nodes
            var adjacentsOfServers = new SortedList<string, string>();
            foreach (var server in _serverGraphInfo.Servers)
            {
                var mdsServer = new MDSServer
                {
                    ServerInfo = server,
                };
                _servers.Add(mdsServer);
                adjacentsOfServers.Add(server.Name, server.Adjacents);
                if (server.Name == _serverGraphInfo.ThisServerName)
                {
                    _thisServer = mdsServer;
                }
            }

            //Set Adjacent servers
            foreach (var mdsServer in _servers)
            {
                //Get adjacent names
                var adjacents = adjacentsOfServers[mdsServer.ServerInfo.Name].Split(',');
                //Add nodes as adjacent
                foreach (var adjacent in adjacents)
                {
                    var trimmedAdjacentName = adjacent.Trim();
                    if (string.IsNullOrEmpty(trimmedAdjacentName))
                    {
                        continue;
                    }

                    var adjacentServer = FindServer(trimmedAdjacentName);
                    if (adjacentServer == null)
                    {
                        continue;
                    }

                    mdsServer.Adjacents.Add(adjacentServer);
                }
            }

            //Create labels for servers
            foreach (var server in _servers)
            {
                server.LabelOfServer = CreateServerLabel(server);
            }

            //Set design properties
            foreach (var server in _serverGraphInfo.Servers)
            {
                var mdsServer = FindServer(server.Name);
                if (mdsServer == null)
                {
                    continue;
                }

                var splittedLocation = server.Location.Split(new[] { ',', ';' });
                mdsServer.LabelOfServer.Left = Convert.ToInt32(splittedLocation[0]);
                mdsServer.LabelOfServer.Top = Convert.ToInt32(splittedLocation[1]);
            }
        }

        /// <summary>
        /// Checks graph if it is a valid graph.
        /// </summary>
        private void CheckGraph()
        {
            //Check if This Server is defined
            if (_thisServer == null || FindServer(_thisServer.ServerInfo.Name) == null)
            {
                throw new MDSException("This server is not defined in the graph.");
            }

            //Return, if there is no server except this server
            if (_servers.Count == 1)
            {
                return;
            }

            //Check if there is an unconnected server exist on graph
            foreach (var server in _servers)
            {
                if (server.Adjacents.Count <= 0)
                {
                    throw new MDSException("MDS Server '" + server.ServerInfo.Name + "' has no connection to other servers on graph.");
                }

                if (!IsThereAPath(_thisServer, server, new List<MDSServer>()))
                {
                    throw new MDSException("There is no path from this MDS server (" +
                                           _thisServer.ServerInfo.Name + ") to '" + server.ServerInfo.Name + "'.");
                }
            }
        }
        
        /// <summary>
        /// Prepares ServerGraphInfo object from graph on screen.
        /// </summary>
        /// <returns>ServerGraphInfo object</returns>
        private ServerGraphInfo CreateServerGraphInfo()
        {
            var graphInfo = new ServerGraphInfo
                                {
                                    ThisServerName = _thisServer.ServerInfo.Name,
                                    Servers = new ServerGraphInfo.ServerOnGraph[_servers.Count]
                                };
            for (var i = 0; i < _servers.Count; i++)
            {
                graphInfo.Servers[i] =
                    new ServerGraphInfo.ServerOnGraph
                        {
                            Name = _servers[i].ServerInfo.Name,
                            IpAddress = _servers[i].ServerInfo.IpAddress,
                            Port = _servers[i].ServerInfo.Port,
                            Location = _servers[i].LabelOfServer.Left + "," + _servers[i].LabelOfServer.Top,
                            Adjacents = GetAdjacentListAsString(_servers[i])
                        };
            }

            return graphInfo;
        }

        /// <summary>
        /// Creates a comma delimited string list from adjacent names of given server.
        /// </summary>
        /// <param name="server">Server to get adjacent list</param>
        /// <returns>Adjacent list as string</returns>
        private static string GetAdjacentListAsString(MDSServer server)
        {
            var adjacentList = new StringBuilder();
            for (var i = 0; i < server.Adjacents.Count; i++)
            {
                if (i > 0)
                {
                    adjacentList.Append(",");
                }

                adjacentList.Append(server.Adjacents[i].ServerInfo.Name);
            }

            return adjacentList.ToString();
        }

        /// <summary>
        /// Checks if there is a path from a MDS server to another.
        /// I runs as recursive and searches all nodes to find a path from source to destination.
        /// </summary>
        /// <param name="sourceServer">Source server of path</param>
        /// <param name="destServer">Destination server of path</param>
        /// <param name="passedServers">List of all passed servers until now</param>
        /// <returns>True, if there is a path</returns>
        private static bool IsThereAPath(MDSServer sourceServer, MDSServer destServer, ICollection<MDSServer> passedServers)
        {
            if (passedServers.Contains(sourceServer))
            {
                return false;
            }

            if (sourceServer.Adjacents.Contains(destServer))
            {
                return true;
            }

            passedServers.Add(sourceServer);
            
            foreach (var adjacent in sourceServer.Adjacents)
            {
                if(IsThereAPath(adjacent, destServer, passedServers))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds a server in Servers list by name.
        /// </summary>
        /// <param name="name">Name of server</param>
        /// <returns>Found server or null if can not find</returns>
        private MDSServer FindServer(string name)
        {
            foreach (var server in _servers)
            {
                if (server.ServerInfo.Name == name)
                {
                    return server;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a server in Servers list by Label.
        /// </summary>
        /// <param name="label">Label of server</param>
        /// <returns>Found server or null if can not find</returns>
        private MDSServer FindServer(Label label)
        {
            foreach (var server in _servers)
            {
                if (server.LabelOfServer == label)
                {
                    return server;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a label for a server.
        /// </summary>
        /// <param name="server">Server</param>
        /// <returns>Label</returns>
        private Label CreateServerLabel(MDSServer server)
        {
            var label = new Label
            {
                BackColor = Color.DodgerBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Verdana", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162),
                ForeColor = Color.White,
                Location = new Point(1, 1),
                Name = "lbl" + server.ServerInfo.Name,
                Size = new Size(180, 40),
                TabIndex = 1,
                Text = server.ServerInfo.Name + Environment.NewLine + server.ServerInfo.IpAddress,
                TextAlign = ContentAlignment.MiddleCenter
            };

            if (server == _thisServer)
            {
                label.BackColor = Color.Crimson;
            }

            label.ContextMenuStrip = cmsServerRightMenu;
            label.MouseDown += ServerLabel_MouseDown;
            label.MouseUp += ServerLabel_MouseUp;
            label.MouseMove += ServerLabel_MouseMove;
            label.MouseDoubleClick += ServerLabel_MouseDoubleClick;

            pnlDesign.Controls.Add(label);

            return label;
        }

        /// <summary>
        /// Searches all labels if any label's left value is 5 pixel close to given x value.
        /// </summary>
        /// <param name="x">x value to search</param>
        /// <returns>int.MinValue: No label found, >int.MinValue: Left value of found label</returns>
        private int FindGuideLabelX(int x)
        {
            foreach (var mdsServer in _servers)
            {
                if (mdsServer.LabelOfServer == _selectedLabel)
                {
                    continue;
                }

                if (Math.Abs(mdsServer.LabelOfServer.Left - x) < 6)
                {
                    return mdsServer.LabelOfServer.Left;
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// Searches all labels if any label's Top value is 5 pixel close to given y value.
        /// </summary>
        /// <param name="y">y value to search</param>
        /// <returns>int.MinValue: No label found, >int.MinValue: Top value of found label</returns>
        private int FindGuideLabelY(int y)
        {
            foreach (var mdsServer in _servers)
            {
                if (mdsServer.LabelOfServer == _selectedLabel)
                {
                    continue;
                }

                if (Math.Abs(mdsServer.LabelOfServer.Top - y) < 6)
                {
                    return mdsServer.LabelOfServer.Top;
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// Links two servers if they are not linked, else unlink them.
        /// </summary>
        /// <param name="firstServerLabel">Label of first server</param>
        /// <param name="secondServerLabel">Label of second server</param>
        private void LinkOrUnlinkServers(Label firstServerLabel, Label secondServerLabel)
        {
            if (firstServerLabel == secondServerLabel)
            {
                return;
            }

            var firstServer = FindServer(firstServerLabel);
            var secondServer = FindServer(secondServerLabel);
            if (firstServer == null || secondServer == null)
            {
                return;
            }

            if (firstServer.Adjacents.Contains(secondServer))
            {
                firstServer.Adjacents.Remove(secondServer);
                if (secondServer.Adjacents.Contains(firstServer))
                {
                    secondServer.Adjacents.Remove(firstServer);
                }
            }
            else
            {
                firstServer.Adjacents.Add(secondServer);
                if (!secondServer.Adjacents.Contains(firstServer))
                {
                    secondServer.Adjacents.Add(firstServer);
                }
            }
        }
        
        /// <summary>
        /// Draws all lines between adjacent servers.
        /// </summary>
        private void DrawAllLines(Graphics graphics)
        {
            if (_servers == null)
            {
                return;
            }

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (_selectedLabel != null && _movingLabelGuideLabelX >= 0)
            {
                graphics.DrawLine(_guideLinePen, _selectedLabel.Left, 0, _selectedLabel.Left, pnlDesign.Height);
                graphics.DrawLine(_guideLinePen, _selectedLabel.Right, 0, _selectedLabel.Right, pnlDesign.Height);
            }

            if (_selectedLabel != null && _movingLabelGuideLabelY >= 0)
            {
                graphics.DrawLine(_guideLinePen, 0, _selectedLabel.Top, pnlDesign.Width, _selectedLabel.Top);
                graphics.DrawLine(_guideLinePen, 0, _selectedLabel.Bottom, pnlDesign.Width, _selectedLabel.Bottom);
            }

            if (_selectedLabel != null)
            {
                graphics.DrawRectangle(
                    _labelSelectingPen,
                    _selectedLabel.Left - 4,
                    _selectedLabel.Top - 4,
                    _selectedLabel.Width + 7,
                    _selectedLabel.Height + 7
                    );
            }

            var passedServers = new List<MDSServer>();
            foreach (var server in _servers)
            {
                passedServers.Add(server);
                foreach (var adjacent in server.Adjacents)
                {
                    if (passedServers.Contains(adjacent))
                    {
                        continue;
                    }

                    graphics.DrawLine(
                        _linePen,
                        server.LabelOfServer.Location.X + server.LabelOfServer.Width / 2,
                        server.LabelOfServer.Location.Y + server.LabelOfServer.Height / 2,
                        adjacent.LabelOfServer.Location.X + adjacent.LabelOfServer.Width / 2,
                        adjacent.LabelOfServer.Location.Y + adjacent.LabelOfServer.Height / 2
                        );
                }
            }
        }
        
        /// <summary>
        /// Finds selected server on design area.
        /// </summary>
        /// <returns>Selected server or null if not selected any server</returns>
        private MDSServer GetSelectedServer()
        {
            return (_selectedLabel == null) ? null : FindServer(_selectedLabel);
        }

        #endregion

        #region Sub classes

        /// <summary>
        /// Represents a MDS server on design area.
        /// </summary>
        private class MDSServer
        {
            /// <summary>
            /// Reference to ServerGraphInfo.ServerOnGraph associated with this MDSServer object.
            /// </summary>
            public ServerGraphInfo.ServerOnGraph ServerInfo { get; set; }

            /// <summary>
            /// List of adjacent servers of MDS server.
            /// </summary>
            public List<MDSServer> Adjacents { get; private set; }

            /// <summary>
            /// Label that represents server on ServerGraphForm form.
            /// </summary>
            public Label LabelOfServer { get; set; }

            /// <summary>
            /// Creates a new MDSServer object.
            /// </summary>
            public MDSServer()
            {
                Adjacents = new List<MDSServer>();
            }
        }
       
        #endregion
    }
}
