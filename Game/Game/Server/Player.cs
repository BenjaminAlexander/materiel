using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Networking;
using MyGame.Utils;
using System.Threading;
using System.Net;
using MyGame.Client;
using MyGame.RtsCommands;
using MyGame.materiel;

namespace MyGame.Server
{
    public abstract class BasePlayer<InUDP, InTCP>
        where InUDP : UdpMessage
        where InTCP : TcpMessage
    {
        private UdpTcpPair client;

        private ThreadSafeQueue<UdpMessage> outgoingUDPQueue = new ThreadSafeQueue<UdpMessage>();
        private ThreadSafeQueue<InUDP> incomingUDPQueue = new ThreadSafeQueue<InUDP>();

        private ThreadSafeQueue<TcpMessage> outgoingTCPQueue = new ThreadSafeQueue<TcpMessage>();
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

        public void SendUDP(UdpMessage message)
        {
            outgoingUDPQueue.Enqueue(message);
        }

        public void SendUDP(Queue<UdpMessage> messages)
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
                    UdpMessage m = outgoingUDPQueue.Dequeue();
                    m.Send(this.client);
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

        public void SendTCP(TcpMessage message)
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
                    TcpMessage message = outgoingTCPQueue.Dequeue();
                    message.Send(client);
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

        public abstract InUDP GetUDPMessage(UdpTcpPair client);

        public abstract InTCP GetTCPMessage(UdpTcpPair client);
    }
}
