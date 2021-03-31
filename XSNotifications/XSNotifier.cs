using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using XSNotifications.Exception;
using XSNotifications.Helpers;

namespace XSNotifications
{
    public class XSNotifier : IDisposable
    {
        private bool isDisposed = false;
        public bool IsDisposed { get => isDisposed; private set { isDisposed = value; } }
        public volatile bool IsRunning = false;
        private volatile bool doThreadTerminate = false;
        private WaitHandle dispatchHandle { get; set; }
        private ConcurrentQueue<XSNotification> queue { get; set; }
        private UdpClient udpClient { get; set; }

        /// <summary>
        /// This constructor binds to any available local port and uses the internal default for the server endpoint as bound on 127.0.0.1.
        /// </summary>
        public XSNotifier() : this(null, null) { }

        /// <summary>
        /// This constructor will bind the client to any available local port.
        /// </summary>
        /// <param name="_serverEndpoint">IPEndPoint for XSOverlay.</param>
        public XSNotifier(IPEndPoint _serverEndpoint) : this(null, _serverEndpoint) { }

        /// <summary>
        /// This constructor will bind the client to the specified local port.
        /// Server port is bound to endpoint on 127.0.0.1
        /// </summary>
        /// <param name="_clientPort">Client port.</param>
        /// <param name="_serverPort">XSOverlay port.</param>
        public XSNotifier(int _clientPort, int _serverPort) : this(new IPEndPoint(IPAddress.Parse("127.0.0.1"), _clientPort), new IPEndPoint(IPAddress.Parse("127.0.0.1"), _serverPort)) { }

        /// <summary>
        /// This constructor will bind to the specified client and server endpoints.
        /// </summary>
        /// <param name="_clientEndpoint">Client endpoint. Null will bind to any available local port.</param>
        /// <param name="_serverEndpoint">IPEndPoint for XSOverlay. Null will bind to 127.0.0.1 on internal default port.</param>
        public XSNotifier(IPEndPoint _clientEndpoint = null, IPEndPoint _serverEndpoint = null)
        {
            try
            {
                if ((_clientEndpoint != null && (_clientEndpoint.Port < 1 || _clientEndpoint.Port > 65535))
                || (_serverEndpoint != null && (_serverEndpoint.Port < 1 || _serverEndpoint.Port > 65535)))
                    throw new XSNetworkException("Specified port out of range! Valid values are 1-65535.");

                if (_serverEndpoint == null)
                    _serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), XSGlobals.DefaultServerPort);

                udpClient = _clientEndpoint == null ? new UdpClient() : new UdpClient(_clientEndpoint);
                queue = new ConcurrentQueue<XSNotification>();
                dispatchHandle = new AutoResetEvent(false);

                ThreadPool.QueueUserWorkItem(new WaitCallback(QueueWorker), this);

                udpClient.Connect(_serverEndpoint);
            }
            catch (System.Exception ex)
            {
                throw new XSNetworkException(ex);
            }
        }

        /// <summary>
        /// Sends a notification from the local udp client to XSOverlay.
        /// </summary>
        /// <param name="notification">Notification object to configure.</param>
        public void SendNotification(XSNotification notification)
        {
            if (!IsDisposed)
            {
                queue.Enqueue(notification);
                ((AutoResetEvent)dispatchHandle).Set();
            }
            else
                throw new System.ObjectDisposedException("XSNotifier object was already disposed of!");
        }

        private static void QueueWorker(object arg)
        {
            XSNotifier instance = (XSNotifier)arg;
            instance.IsRunning = true;

            try
            {
                while (instance.dispatchHandle.WaitOne())
                {
                    if (instance.doThreadTerminate) break;

                    while (!instance.queue.IsEmpty)
                    {
                        XSNotification nextNotification;

                        while (!instance.queue.TryDequeue(out nextNotification)) Task.Delay(10).GetAwaiter().GetResult();

                        byte[] dgram = nextNotification.AsJsonBytes();
                        instance.udpClient.Send(dgram, dgram.Length);
                    }
                }
            }
            catch (System.Exception ex)
            {
                instance.IsRunning = false;
                throw new XSRuntimeException("An exception occurred in the queue worker thread. See inner exception for details.", ex);
            }
        }

        /// <summary>
        /// Cleans up threads and other resources used by the notification system.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                int maxWaitAttempt = 10;
                int waitAttempt = 0;
                int maxThreads, availableThreads;
                while (true)
                {
                    if (maxWaitAttempt == waitAttempt)
                        break;

                    doThreadTerminate = true;
                    ((AutoResetEvent)dispatchHandle).Set();

                    ThreadPool.GetAvailableThreads(out availableThreads, out _);
                    ThreadPool.GetMaxThreads(out maxThreads, out _);

                    if (availableThreads == maxThreads) break;

                    Task.Delay(50).GetAwaiter().GetResult();
                    ++waitAttempt;
                }
            }
        }
    }
}
