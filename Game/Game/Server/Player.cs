using System;
using System.Collections.Generic;
using MyGame.Networking;
using MyGame.Utils;
using System.Threading;
using System.Net;
using System.Reflection;
using System.Net.Sockets;

namespace MyGame.Server
{
    public abstract class BasePlayer<InUDP, InTCP>
        where InUDP : GameMessage
        where InTCP : GameMessage
    {
        private UdpTcpPair client;

        private ConstructorInfo udpConstructor;
        private ConstructorInfo tcpConstructor;

        private ThreadSafeQueue<GameMessage> outgoingUDPQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<InUDP> incomingUDPQueue = new ThreadSafeQueue<InUDP>();

        private ThreadSafeQueue<GameMessage> outgoingTCPQueue = new ThreadSafeQueue<GameMessage>();
        private ThreadSafeQueue<InTCP> incomingTCPQueue = new ThreadSafeQueue<InTCP>();

        private Thread outboundUDPSenderThread;
        private Thread inboundUDPReaderThread;

        private Thread outboundTCPSenderThread;
        private Thread inboundTCPReaderThread;

        public BasePlayer()
        {
            this.client = new UdpTcpPair();
            this.StartThreads();
        }

        public BasePlayer(IPAddress serverAddress)
        {
            this.client = new UdpTcpPair(serverAddress);
            this.StartThreads();
        }

        private void StartThreads()
        {
            Type[] udpParams = new Type[1];
            udpParams[0] = typeof(UdpClient);

            Type[] tcpParams = new Type[1];
            tcpParams[0] = typeof(NetworkStream);

            udpConstructor = typeof(InUDP).GetConstructor(udpParams);
            tcpConstructor = typeof(InTCP).GetConstructor(tcpParams);

            if (udpConstructor == null || tcpConstructor == null)
            {
                throw new Exception("bad message types");
            }

            this.inboundUDPReaderThread = new Thread(new ThreadStart(InboundUDPReader));
            this.inboundUDPReaderThread.Start();

            this.outboundUDPSenderThread = new Thread(new ThreadStart(OutboundUDPSender));
            this.outboundUDPSenderThread.Start();

            this.outboundTCPSenderThread = new Thread(new ThreadStart(OutboundTCPSender));
            this.outboundTCPSenderThread.Start();

            this.inboundTCPReaderThread = new Thread(new ThreadStart(InboundTCPReader));
            this.inboundTCPReaderThread.Start();
        }

        public virtual void Disconnect()
        {
            this.outboundUDPSenderThread.Abort();
            this.inboundUDPReaderThread.Abort();
            this.outboundTCPSenderThread.Abort();
            this.inboundTCPReaderThread.Abort();
            this.client.Disconnect();
        }

        public void SendUDP(GameMessage message)
        {
            outgoingUDPQueue.Enqueue(message);
        }

        public void SendUDP(Queue<GameMessage> messages)
        {
            outgoingUDPQueue.EnqueueAll(messages);
        }

        public Queue<InUDP> DequeueAllIncomingUDP()
        {
            return incomingUDPQueue.DequeueAll();
        }

        private void OutboundUDPSender()
        {
            try
            {
                while (true)
                {
                    GameMessage m = outgoingUDPQueue.Dequeue();
                    m.SendUDP(this.client);
                }
            }
            catch (Exception)
            {
                //TODO: notify the caller somehow
            }
        }

        private void InboundUDPReader()
        {
            try
            {
                while (true)
                {
                    InUDP m = this.GetUDPMessage(this.client);
                    incomingUDPQueue.Enqueue(m);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }

        public void SendTCP(GameMessage message)
        {
            outgoingTCPQueue.Enqueue(message);
        }

        public InTCP DequeueIncomingTCP()
        {
            return incomingTCPQueue.Dequeue();
        }

        public Queue<InTCP> DequeueAllIncomingTCP()
        {
            return incomingTCPQueue.DequeueAll();
        }

        private void OutboundTCPSender()
        {
            try
            {
                while (true)
                {
                    GameMessage message = outgoingTCPQueue.Dequeue();
                    message.SendTCP(client);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }

        private void InboundTCPReader()
        {
            try
            {
                while (true)
                {
                    InTCP m = this.GetTCPMessage(this.client);
                    incomingTCPQueue.Enqueue(m);
                }
            }
            catch (Exception)
            {
                //TODO: close the client game
            }
        }

        public int Id
        {
            get
            {
                return client.Id;
            }
        }

        public InUDP GetUDPMessage(UdpTcpPair client)
        {
            object[] constuctorParams = new object[1];
            constuctorParams[0] = client.UdpClient;
            return (InUDP)udpConstructor.Invoke(constuctorParams);
        }

        public InTCP GetTCPMessage(UdpTcpPair client)
        {
            object[] constuctorParams = new object[1];
            constuctorParams[0] = client.ClientStream;
            return (InTCP)tcpConstructor.Invoke(constuctorParams);
        }
    }
}
