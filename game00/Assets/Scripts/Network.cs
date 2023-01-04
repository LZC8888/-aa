using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class Network : MonoBehaviour
{
    const byte REGIST_REQUEST = 1;
    const byte REGIST_FAIL = 2;
    const byte REGIST_SUCCESS = 3;
    const byte LOGIN_REQUEST = 4;
    const byte LOGIN_FAIL_USERDONTEXIST = 5;
    const byte LOGIN_FAIL_PASSWORDWRONG = 6;
    const byte LOGIN_FAIL_ALREADYLOGIN = 7;
    const byte LOGIN_SUCCESS = 8;
    Socket clientSock;
    Text inputText, outputText, inputPassword;
    public int iSceneNum=0;
    List<string> msgList = new List<string>();
    public string userName;
    String recvStr;
    // Start is called before the first frame update
    void Start()
    {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        inputText = GameObject.Find("Canvas/InputField/Text").GetComponent<Text>();
        outputText = GameObject.Find("Canvas/outputText").GetComponent<Text>();
        inputPassword = GameObject.Find("Canvas/PasswordInput/Text").GetComponent<Text>();

        GameObject nobj = GameObject.Find("GameObject");//�л�����ʱ���ᶪʧGameobject
        DontDestroyOnLoad(this);
        iSceneNum = 0;
    }
    int flag = 0;
    byte[] sendBuff = new byte[100];
    byte[] recvBuff = new byte[100];

    bool connected = false;
    public void ConnectBtnClicked()
    {
        try
        {
            clientSock.Connect("127.0.0.1", 10001);
            clientSock.BeginReceive(recvBuff, 0, 100, 0, recvCb, clientSock);
            connected = true;
            
        }
        catch (Exception e)
        {
            Console.WriteLine("no server");
        }
    }
    public Socket GetSocket()
    {
        return clientSock;
    }
    void recvCb(IAsyncResult iar)
    {
        Socket tempSock = (Socket)iar.AsyncState;
        int recvNum = tempSock.EndReceive(iar);
        switch (recvBuff[0])
        {
            case REGIST_SUCCESS:
                recvStr = "ע��ɹ�";
                break;
            case REGIST_FAIL:
                recvStr = "�û����Ѵ���";
                break;
            case LOGIN_FAIL_ALREADYLOGIN:
                recvStr = "���û��ѵ�¼";
                break;
            case LOGIN_FAIL_PASSWORDWRONG:
                recvStr = "�������";
                break;
            case LOGIN_FAIL_USERDONTEXIST:
                recvStr = "�û�������";
                break;
            case LOGIN_SUCCESS:
                recvStr = "��¼�ɹ�";//��¼��ת�����䳡��
                flag = 1;
                break;
        }
        if(iSceneNum==0)
            tempSock.BeginReceive(recvBuff, 0, 100, 0, recvCb, tempSock);
    }
    public void SendBtnClicked()
    {
        string inputStr = inputText.text;
        var spaceIndex = inputStr.IndexOf(" ");
        if (spaceIndex!=-1) { recvStr = "�û������ڿո�"; return; }
        String passwordMD5Str = GetMD5String(inputPassword.text);
        String sendStr = "registRequest " + inputText.text + " " + passwordMD5Str;
        sendBuff = System.Text.Encoding.Default.GetBytes(sendStr);
        //   sendBuff[0] = REGIST_REQUEST;
        
        clientSock.Send(sendBuff);
    }
    private String GetMD5String(String password)
    {
        MD5 md5Text = MD5.Create();
        byte[] temp = md5Text.ComputeHash(System.Text.Encoding.Default.GetBytes(password));
        String md5Str = "";
        for (int i = 0; i < temp.Length; i++) md5Str += temp[i].ToString("X2");
        return md5Str;
    }
    public void LoginBtnClicked()
    {
      //  SceneManager.LoadScene(1);
        String passwordMD5Str = GetMD5String(inputPassword.text);
        String sendStr = "loginRequest " + inputText.text + " " + passwordMD5Str;
        sendBuff = System.Text.Encoding.Default.GetBytes(sendStr);
        userName = inputText.text;
       // sendBuff[0] = LOGIN_REQUEST;
        clientSock.Send(sendBuff);
    }

    //��ȡclientSock���󣨹��ڶ���������ȡsocket�����ã�
    public Socket GetClientSock()
    {
        return clientSock;
    }

    // Update is called once per frame

    void SendHeartbeat()
    {
        clientSock.Send(System.Text.Encoding.Default.GetBytes("heartBeat "));//����������
    }
    int i = 0;
    void Update()
    {
         outputText.text =recvStr ;
     //   outputText.text = IP;
        i++;
        if (i % 240 == 0 && connected)
            SendHeartbeat();
        if (flag == 1)
        {
            flag = 0;
            SceneManager.LoadScene(1);//��ת�³���
        }
    }
    
}
