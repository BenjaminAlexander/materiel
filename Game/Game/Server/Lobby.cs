using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using MyGame;
using MyGame.Networking;
using MyGame.Utils;
using MyGame.RtsCommands;

namespace MyGame.Server
{
    public class Lobby
    {
        private volatile bool clientsLocked = false;
        private List<RemotePlayer> clients = new List<RemotePlayer>();
        private Mutex clientsMutex = new Mutex(false);

        public List<RemotePlayer> Clients
        {
            get
            {
                return clients;
            }
        }

        // Adds a client to the current clientlist. Returns true if the client is added, returns false if the clients are locked.
        private bool AddClient(RemotePlayer client)
        {
            bool added = false;
            clientsMutex.WaitOne();

            if (!clientsLocked)
            {
                clients.Add(client);
                added = true;
            }

            clientsMutex.ReleaseMutex();
            return added;
        }

        //This method starts running the lobby and blocks until the lobby ends
        public void Run()
        {
            //Start the client listener in its own thread
            //This thread allows clients to asynchronously join the lobby
            Thread clientListenerThread = new Thread(new ThreadStart(ClientListener));
            clientListenerThread.Start();

            //wait to start
            MessageBox.Show("Enter to start", NetUtils.GetLocalIP());

            //lock client list
            clientsMutex.WaitOne();
            clientsLocked = true;
            clientsMutex.ReleaseMutex();

            // Start up the game.
            ServerGame game = new ServerGame(this);
            game.Run();

            //the game has finished
            foreach (RemotePlayer c in clients)
            {
                c.Disconnect();
            }

            //this.prelimListener.Stop();
            UdpTcpPair.StopListener();
            clientListenerThread.Join();
        }

        public void BroadcastUDP(GameMessage message)
        {
            foreach (RemotePlayer client in clients)
            {
                client.SendUDP(message);
            }
        }

        public void BroadcastUDP(Queue<GameMessage> messages)
        {
            foreach (RemotePlayer client in clients)
            {
                client.SendUDP(messages);
            }
        }

        public void BroadcastTCP(GameMessage message)
        {
            foreach (RemotePlayer client in clients)
            {
                client.SendTCP(message);
            }
        }

        public void HandleAllTCPMessages(ServerGame game)
        {
            foreach (RemotePlayer client in clients)
            {
                client.HandleAllTCPMessages(game);
            }
        }

        private void ClientListener()
        {
            UdpTcpPair.StartListener();
            try
            {
                while (true)
                {
                    RemotePlayer clientobj = new RemotePlayer();
                    this.AddClient(clientobj);
                }
            }
            catch (Exception) { /*Just let the listener thread end*/ }
        }
    }
}
