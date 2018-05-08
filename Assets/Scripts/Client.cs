using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client {

    private TcpClient clientSocket;
    private IPAddress ipAddress;
    private IPHostEntry ipHostInfo;
    private NetworkStream serverStream;
    private byte[] outStream;
    private byte[] inStream;
    private bool connection = false;

    public void Start () {
        clientSocket = new TcpClient();
    }

    public void connect()
    {
        ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        ipAddress = ipHostInfo.AddressList[0];
        clientSocket.Connect(ipAddress, 1234);
        connection = true;
    }

    public void send(string name, int score)
    {
        serverStream = clientSocket.GetStream();
        outStream = Encoding.ASCII.GetBytes(name);
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();
        outStream = Encoding.ASCII.GetBytes(score.ToString());
        serverStream.Write(outStream, 0, outStream.Length);
        serverStream.Flush();
    }

    public String read()
    {
        inStream = new byte[100];
        serverStream.Read(inStream, 0, inStream.Length);
        string data = System.Text.Encoding.ASCII.GetString(inStream);
        return data;
    }

    public bool getConnection()
    {
        return connection;
    }
}
