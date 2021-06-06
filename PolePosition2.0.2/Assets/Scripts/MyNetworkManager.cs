using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public List<SetupPlayer> listaInfoJugadores;


    #region Client

    public override void OnClientConnect(NetworkConnection conn){
           base.OnClientConnect(conn);

            Debug.Log("Cliente conectado");
        }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("StopClient");
        GameObject.Find("UIManager").GetComponent<UIManager>().ActivateMainMenu();


        Camera.main.gameObject.GetComponent<Transform>().position = Camera.main.gameObject.GetComponent<CameraController>().posInicial;
        Camera.main.gameObject.GetComponent<Transform>().rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        
    }

    public override void OnStopServer(){
        base.OnStopServer();
        Debug.Log("StopServer");

    }

    public override void OnServerDisconnect(NetworkConnection conn){
        Debug.Log("ServerDisconnect");
    }

    /*  public override void AddPlayer(){

      }*/

    #endregion

    #region Server

    public override void OnServerConnect(NetworkConnection conn){
            Debug.Log("Nueva connección okay");
        }
        
        [Server]
        private void AddPlayerToList(SetupPlayer info)
        {
        listaInfoJugadores.Add(info);
        }

    #endregion

}
