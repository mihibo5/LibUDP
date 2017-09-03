using LibUDP.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

///----------------------------------------------------------------------
///   Namespace:      LibUDP
///   Class:          ServerUDP
///   Description:    UDP Server
///   Author:         Miha Bogataj              Date: 2017.08.24
///   Notes:          /
///   Revision History:
///   Name: 1.0.0     Date: 2017.08.24  Description: UDP Server
///----------------------------------------------------------------------

namespace LibUDP
{
    public class ServerUDP
    {
        #region "EVENTS"
        //Message recieved event      
        public delegate void MessageRecievedHandler(object sender, MessageRecievedArgs e);
        [Category("Message")]
        [Description("Fires when message is recieved.")]
        public event MessageRecievedHandler MessageRecieved;

        //Connection established event
        public delegate void ConnectionEstablishedEventHandler(object sender, ConnectionEventArgs e);
        [Category("Connection")]
        [Description("Fires when connection is established.")]
        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        //Connection lost event 
        public delegate void ConnectionLostEventHandler(object sender, ConnectionEventArgs e);
        [Category("Connection")]
        [Description("Fires when connection is lost (not yet implemented).")]
        public event ConnectionLostEventHandler ConnectionLost;
        #endregion

        //Server variables
        [Category("Address")]
        [Description("Sets server port.")]
        public int Port { get; set; }

        #region "VARIABLES"
        private UdpClient receiver;

        //Connection list
        private List<string> connections = new List<string>();
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">Server port</param>
        public ServerUDP(int port)
        {
            try
            {
                this.Port = port;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Server initialization
        /// </summary>
        private void ServerStart()
        {         
            try
            {
                receiver = new UdpClient(this.Port);
                receiver.BeginReceive(DataReceived, receiver);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Starts server
        /// </summary>
        /// <returns>Whether server start was successful</returns>
        public bool Start()
        {
            try
            {
                this.ServerStart();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Stops server
        /// </summary>
        public void Stop()
        {
            try
            {
                receiver.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recieves data
        /// </summary>
        /// <param name="ar">recieved data</param>
        private void DataReceived(IAsyncResult ar)
        {
            try
            {
                UdpClient c = (UdpClient)ar.AsyncState;
                IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                if (c.Client != null)
                {
                    while (c.Client == null) ;
                    Byte[] receivedBytes = c.EndReceive(ar, ref receivedIpEndPoint);


                    //Connection established event handling
                    if (!connections.Contains(receivedIpEndPoint.Address.ToString()))
                    {
                        connections.Add(receivedIpEndPoint.Address.ToString());
                        OnConnectionEstablished(receivedIpEndPoint);
                    }

                    // Convert data to ASCII and print in console
                    string receivedText = ASCIIEncoding.ASCII.GetString(receivedBytes);

                    //Not needed due to an event
                    //Console.Write(receivedIpEndPoint + ": " + receivedText + Environment.NewLine);

                    // Message recieved event handling
                    OnMessageRecieved(receivedText, receivedIpEndPoint);

                    // Restart listening for udp data packages
                    c.BeginReceive(DataReceived, ar.AsyncState);

                    // Return data
                    this.Send("READBACK: " + receivedText, receivedIpEndPoint);

                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Send message to client
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="ip">client address</param>
        public void Send(string message, IPEndPoint ip)
        {
            try
            {
                byte[] messageByte = Encoding.ASCII.GetBytes(message);
                receiver.Send(messageByte, messageByte.Length, ip);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region "EVENTS"
        /// <summary>
        /// On message recieved event
        /// </summary>
        /// <param name="recievedText">recieved text</param>
        /// <param name="end">client address</param>
        protected virtual void OnMessageRecieved(string recievedText, IPEndPoint end)
        {
            try
            {
                MessageRecieved?.Invoke(this, new MessageRecievedArgs()
                {
                    Text = recievedText,
                    Address = end.ToString(),
                    TimeRecieved = DateTime.Now
                });
            }
            catch
            {
                throw;
            }        
        }

        /// <summary>
        /// On connection established event
        /// </summary>
        /// <param name="end">client address</param>
        protected virtual void OnConnectionEstablished(IPEndPoint end)
        {
            try
            {
                ConnectionEstablished?.Invoke(this, new ConnectionEventArgs()
                {
                    Address = end.ToString(),
                    ConnectionTime = DateTime.Now
                });
            }
            catch
            {
                throw;
            }          
        }

        /// <summary>
        /// On connection lost event
        /// </summary>
        /// <param name="end">sender address</param>
        protected virtual void OnConnectionLost(IPEndPoint end)
        {
            try
            {
                ConnectionLost?.Invoke(this, new ConnectionEventArgs()
                {
                    Address = end.ToString(),
                    ConnectionTime = DateTime.Now
                });
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
