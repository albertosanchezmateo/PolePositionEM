using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{


    #region Client
        public override void OnClientConnect(NetworkConnection conn){
           //base.OnClientConnect(conn);

            Debug.Log("Cliente conectado");
        }

      /*  public override void AddPlayer(){

        }*/

    #endregion

    #region Server

        public override void OnServerConnect(NetworkConnection conn){
            Debug.Log("Nueva connección okay");
        }


    #endregion

}
