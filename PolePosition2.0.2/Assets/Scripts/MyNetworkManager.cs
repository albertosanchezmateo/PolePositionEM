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
