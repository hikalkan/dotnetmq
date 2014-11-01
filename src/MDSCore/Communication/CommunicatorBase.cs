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

using MDS.Communication.Events;
using MDS.Communication.Messages;
using MDS.Exceptions;

namespace MDS.Communication
{
    /// <summary>
    /// This is the case class for all communicators.
    /// A communicator can also be directly implement ICommunicator,
    /// but this class helps to build a communicator class with several helper ana generic methods/properties. 
    /// </summary>
    public abstract class CommunicatorBase : ICommunicator
    {
        #region Events

        /// <summary>
        /// This event is raised when the state of the communicator changes.
        /// </summary>
        public event CommunicatorStateChangedHandler StateChanged;

        /// <summary>
        /// This event is raised when a MdsMessage received.
        /// </summary>
        public event MessageReceivedFromCommunicatorHandler MessageReceived;

        #endregion

        #region Public properties

        /// <summary>
        ///  Unique identifier for this communicator.
        /// </summary>
        public long ComminicatorId { get; private set; }

        /// <summary>
        /// Communication way for this communicator.
        /// </summary>
        public CommunicationWays CommunicationWay { get; set; }

        /// <summary>
        /// Connection state of communicator.
        /// </summary>
        public CommunicationStates State
        {
            get { return _state; }
        }
        private volatile CommunicationStates _state;

        #endregion

        #region Private fields

        /// <summary>
        /// Used to send only one message in a time by locking.
        /// </summary>
        private readonly object _sendLock;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="comminicatorId">Unique identifier for this communicator.</param>
        protected CommunicatorBase(long comminicatorId)
        {
            ComminicatorId = comminicatorId;
            CommunicationWay = CommunicationWays.Send;
            _state = CommunicationStates.Closed;
            _sendLock = new object();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Starts communication.
        /// </summary>
        public void Start()
        {
            if (State != CommunicationStates.Closed)
            {
                throw new MDSException("Communicator is already connected");
            }

            lock (_sendLock)
            {
                OnChangeState(CommunicationStates.Connecting);
                StartCommunicaiton();
                OnChangeState(CommunicationStates.Connected);
            }
        }

        /// <summary>
        /// Stops communication.
        /// </summary>
        public void Stop(bool waitToStop)
        {
            if (State != CommunicationStates.Connected)
            {
                return;
            }

            OnChangeState(CommunicationStates.Closing);
            try
            {
                StopCommunicaiton(waitToStop);
            }
            finally
            {
                OnChangeState(CommunicationStates.Closed);
            }
        }

        /// <summary>
        /// Sends a message to the communicator.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(MDSMessage message)
        {
            lock (_sendLock)
            {
                SendMessageInternal(message);                
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Changes the state of the communicator and raises event
        /// </summary>
        /// <param name="newState">New state</param>
        protected void OnChangeState(CommunicationStates newState)
        {
            var oldState = _state;
            _state = newState;
            if (StateChanged != null)
            {
                StateChanged(this, new CommunicatorStateChangedEventArgs {Communicator = this, OldState = oldState});
            }
        }

        /// <summary>
        /// When a MDSMessage received, this method is called from derived class.
        /// </summary>
        /// <param name="message">incoming message from communicator</param>
        protected void OnMessageReceived(MDSMessage message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new MessageReceivedFromCommunicatorEventArgs {Communicator = this, Message = message});
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Derived class must override this method to start the communication.
        /// </summary>
        protected abstract void StartCommunicaiton();

        /// <summary>
        /// Derived class must override this method to stop the communication.
        /// </summary>
        protected abstract void StopCommunicaiton(bool waitToStop);

        /// <summary>
        /// Waits communicator to finish it's job.
        /// </summary>
        public abstract void WaitToStop();

        /// <summary>
        /// Derived class must override this method to send a message.
        /// </summary>
        protected abstract void SendMessageInternal(MDSMessage message);

        #endregion
    }
}
