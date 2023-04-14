using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ARFCon;

namespace ARFLib {
    public class SockListener {
        private ISignListener _ui;
        //protected const string _ack = "<|ACK|>";
        bool _killing = false;
        private Socket? _listener;

        public SockListener(ISignListener ui) {
            _ui = ui;
        }
        public void Kill() {
            _listener?.Close();
            _listener = null;
        }
        public async Task<bool> ListenOnce(string address) {
            var rv = false;
            var local = new IPEndPoint(IPAddress.Any, int.Parse(address));
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                _listener.Blocking = false;
                _listener.Bind(local);
                _listener.Listen(100);
                _ui.Log("Listening on " + address);
                var handler = await _listener.AcceptAsync();
                var buffer = new byte[1024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                _ui.Log(response);
                var newState = await _ui.StateChange(SignState.Parse(response));
                var ackBytes = Encoding.UTF8.GetBytes(newState.Serialize());
                await handler.SendAsync(ackBytes, SocketFlags.None);
                rv = true;
            }
            catch (Exception ex) {
                _ui.Log($"Error for {address} {ex.Message}");
                rv = false;
            }
            finally {
                _listener?.Close();
            }
            _ui.Log("Done listening for " + address);
            return rv;
        }
    }
    public class SockSender {
        private IUi _ui;
        private int _address;
        //protected const string _ack = "<|ACK|>";

        public SockSender(IUi ui, string address)
        {
            _ui = ui;
            _address = int.Parse(address);
        }

        public async Task<SignState> Send(SignState signState) {
            var endpoint = new IPEndPoint(IPAddress.Loopback, _address);
            Socket client = null;
            try {
                client = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                await client.ConnectAsync(endpoint);

                var msg = signState.Serialize();
                var bytes = Encoding.UTF8.GetBytes(msg);
                _ = await client.SendAsync(bytes, SocketFlags.None);
                _ui.Log("Wrote " + msg);

                var buffer = new byte[26];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                var result = SignState.Parse(response);
                return result;
            }
            catch (Exception ex) {
                return new SignState(SignEnum.Error, Config.ErrorColor, ex.Message);
            }
            finally {
                client?.Close();
            }
        }
    }
}
