using System.Net.Sockets;

///-------------------------------------------------------------------------
///   Namespace:      LibUDP.UserControls
///   Class:          ServerUDP
///   Description:    UDP Server User Control
///   Author:         Miha Bogataj              Date: 2017.09.02
///   Notes:          Extension class
///   Revision History:
///   Name: 1.0.0     Date: 2017.09.02  Description: Extension class created
///-------------------------------------------------------------------------
namespace LibUDP
{
    public static class Extension
    {
        /// <summary>
        /// Starts server thread
        /// </summary>
        /// <returns></returns>
        public static bool Start(this UserControls.ServerUDP server)
        {
            try
            {
                server.ServerStart();
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Stops server thread
        /// </summary>
        public static void Stop(this UserControls.ServerUDP server)
        {
            try
            {
                server.receiver.Close();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Checks whether client is connected
        /// </summary>
        /// <returns>connection state of client</returns>
        public static bool IsConnected(this UserControls.ServerUDP server)
        {
            try
            {
                return !(
                    (server.receiver.Client.Poll(1000, SelectMode.SelectRead) && 
                    (server.receiver.Client.Available == 0)) || 
                    !server.receiver.Client.Connected
                );
            }
            catch
            {
                return false;
            }
        }
    }
}
