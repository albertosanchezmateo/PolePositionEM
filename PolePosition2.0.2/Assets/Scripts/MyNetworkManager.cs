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
        }

    // Al parar o desconectar un cliente se establece de nuevo el menú principal.
    public override void OnStopClient()
    {   Camera.main.gameObject.GetComponent<Transform>().position = Camera.main.gameObject.GetComponent<CameraController>().posInicial;
        Camera.main.gameObject.GetComponent<Transform>().rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        
        base.OnStopClient();
        
        Debug.Log("StopClient");
        GameObject.Find("UIManager").GetComponent<UIManager>().ActivateMainMenu();


        
    }

    // Al parar o desconectar el servidor se reestablece la cámara.
    public override void OnStopServer(){
        base.OnStopServer();
        
        Camera.main.gameObject.GetComponent<Transform>().position = Camera.main.gameObject.GetComponent<CameraController>().posInicial;
        Camera.main.gameObject.GetComponent<Transform>().rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

    }

    // Al desconectar un cliente que no sea el host se actualiza el ranking de la partida.
    public override void OnServerDisconnect(NetworkConnection conn){
        Debug.Log(conn.identity.GetComponent<SetupPlayer>().getName());
        GameObject.Find("@PolePositionManager").GetComponent<PolePositionManager>().EliminatePlayerByName(conn.identity.GetComponent<SetupPlayer>().getName());
        base.OnServerDisconnect(conn);

    }
    #endregion

    #region Server

    public override void OnServerConnect(NetworkConnection conn){
    }
        
        [Server]
        private void AddPlayerToList(SetupPlayer info)
        {
        listaInfoJugadores.Add(info);
        }
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            
        }

    #endregion

}
