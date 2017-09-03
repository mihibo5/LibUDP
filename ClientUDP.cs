using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LibUDP
{
    public class ClientUDP
    {
        private string hostName;
        private int recieverPort;
        private int senderPort;

        private UdpClient client;

        public ClientUDP(string IP, int hostPort, int clientPort)
        {
            try
            {
                hostName = IP;
                recieverPort = hostPort;
                senderPort = clientPort;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Connect()
        {
            try
            {
                client = new UdpClient(senderPort);
                client.Connect(hostName, recieverPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }         
        }

        public void Disconnect()
        {
            try
            {
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Send(string message)
        {
            try
            {
                byte[] messageByte = Encoding.ASCII.GetBytes(message);
                client.Send(messageByte, messageByte.Length);

                //client.BeginReceive(DataReceived, client);

                IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Parse(hostName), recieverPort);
                string receivedText = ASCIIEncoding.ASCII.GetString(client.Receive(ref receivedIpEndPoint));
                Console.WriteLine(receivedIpEndPoint + ": " + receivedText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }          
        }

        private void DataReceived(IAsyncResult ar)
        {
            try
            {
                UdpClient c = (UdpClient) ar.AsyncState;

                IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Parse(hostName), recieverPort);
                string receivedText = ASCIIEncoding.ASCII.GetString(client.Receive(ref receivedIpEndPoint));
                Console.WriteLine(receivedText);

                // Restart listening for udp data packages
                c.BeginReceive(DataReceived, ar.AsyncState);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
