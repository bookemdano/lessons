using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ARFLib {
    public class SockListener {
        private ISignListener _ui;
        private Socket _handler;
        protected const string _ack = "<|ACK|>";
        bool _killing = false;
        private Socket? _listener;

        public SockListener(ISignListener ui) {
            _ui = ui;
        }
        
        public async Task Listen(int address) {
            if (_listener != null) {
                _killing = true;
                _listener.Close();
                while (_listener != null)
                    await Task.Delay(50);
                _killing = false;

            }
            var local = new IPEndPoint(IPAddress.Any, address);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                _listener.Blocking = false;
                _listener.Bind(local);
                _listener.Listen(100);
                _ui.Log("Listening on " + address);
                _handler = await _listener.AcceptAsync();
                while (true) {
                    var buffer = new byte[1024];
                    var received = await _handler.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, received);
                    if (string.IsNullOrEmpty(response)) {
                        if (_killing)
                            break;
                        await Task.Delay(100);
                    }
                    else {
                        _ui.Log(response);
                        _ui.StateChange(SignState.Parse(response));
                        var ackBytes = Encoding.UTF8.GetBytes(_ack);
                        await _handler.SendAsync(ackBytes, SocketFlags.None);
                    }
                }
            }
            catch (Exception ex) {
                _ui.Log($"Error for {address} {ex.Message}");
            }
            finally {
                _listener.Close();
                _listener = null;
            }
            _ui.Log("Done listening for " + address);
        }
    }
    public class SockSender {
        private IUi _ui;
        private int _address;
        private Socket _client;
        protected const string _ack = "<|ACK|>";

        public SockSender(IUi ui, string address)
        {
            _ui = ui;
            _address = int.Parse(address);
        }

        public async Task<bool> Send(SignState signState) {
            var endpoint = new IPEndPoint(IPAddress.Loopback, _address);
            try {
                if (_client == null) {
                    _client = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    await _client.ConnectAsync(endpoint);
                }
                var msg = signState.Serialize();
                var bytes = Encoding.UTF8.GetBytes(msg);
                _ = await _client.SendAsync(bytes, SocketFlags.None);
                _ui.Log("Wrote " + msg);

                var buffer = new byte[8];
                var received = await _client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == _ack) {
                    _ui.Log("Acked " + msg);
                    return true;
                }
            }
            catch (Exception) {
                return false;
            }
            return false;
        }
    }
}
