# Irc
A portable class library containing components for communicating over the Internet Relay Chat (IRC) protocol, with support for the Client-to-Client (CTCP) and Direct Client-to-Client (DCC) protocols, and various unofficial extensions to the official IRC protocol.  This library has been used successfully to develop chat client applications for both Windows Desktop and Windows Phone (see connection examples below).

The design goal of this library is to provide developers all of the components necessary to communicate with an IRC server without being burdened with managing or understanding the protocol itself.  The IrcClient class is the primary interface and attempts to completely hide all of the underlying protocol details, leaving developers with a (hopefully) more intuitive API.  IrcClient contains an exhaustive number of events, but this allows application components to register to the events they care about rather than handle and check every incoming message.

Developers are free to extend the IrcClient class to add additional or missing functionality, or implement their own client using the remaining library components as they find convenient.


## Connection Examples
IrcClient requires a component implementing the Irc.IConnection interface.  The following are code samples demonstrating how objects implementing that interface may establish a connection.  This code is not complete and is for reference only.

### Windows Desktop
```
using (_connection = new TcpClient())
{
    _connection.ReceiveTimeout = 300000;
    _connection.SendTimeout = 20000;
    _connection.Connect(RemoteAddress, Port);

    StreamReader input = null;

    if (IsUsingSSL)
    {
        //initialize System.Net.Security.SslStream and create input/output streams
    }
    else
    {
        input = new StreamReader(_connection.GetStream());
        _output = new StreamWriter(_connection.GetStream());
    }

    IsConnected = true;

    string line;
    do
    {
        line = input.ReadLine();
        
        if (!string.IsNullOrWhiteSpace(line))
        {
            MessageReceived.Raise(this, new IrcMessageEventArgs(IrcMessage.Parse(line)));
        }
    }
    while (line != null);
}
```

### Windows Phone
```
Uri _ircAddress = null; //initialize
_connection = new StreamSocket();
_connection.Control.OutboundBufferSizeInBytes = 512;
if (!IsValidatatingSSLCertificates)
{
    _connection.Control.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
}

await _connection.ConnectAsync(new Windows.Networking.HostName(_ircAddress.Host),
                               _ircAddress.Port.ToString(),
                              _ircAddress.Scheme == IrcUriSchemes.Ssl ? SocketProtectionLevel.Tls12 : 
                                                                        SocketProtectionLevel.PlainSocket);
StreamReader inputStream = new StreamReader(_connection.InputStream.AsStreamForRead());
IsConnected = true;

string line;
do
{
    line = await inputStream.ReadLineAsync();
    if (line != null)
    {
        MessageReceived.Raise(this, new IrcMessageEventArgs(IrcMessage.Parse(line)));
    }
}
while (line != null);
```
