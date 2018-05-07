using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client {

    private TcpClient clientSocket;
    private IPAddress ipAddress;
    private IPHostEntry ipHostInfo;
    private NetworkStream serverStream;
    private byte[] outStream;
    private byte[] inStream;

    public void Start () {
        clientSocket = new TcpClient();
    }

    public void connect()
    {
        ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        ipAddress = ipHostInfo.AddressList[0];
        clientSocket.Connect(ipAddress, 1234);
    }

    public void send()
    {
        serverStream = clientSocket.GetStream();
        outStream = System.Text.Encoding.ASCII.GetBytes("test");
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();

        inStream = new byte[10025];
        serverStream.Read(inStream, 0, inStream.Length);
        string returndata = System.Text.Encoding.ASCII.GetString(inStream);
        Debug.Log(returndata);
    }
}
