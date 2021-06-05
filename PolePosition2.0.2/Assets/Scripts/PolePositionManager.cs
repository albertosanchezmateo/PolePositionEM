using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PolePositionManager : NetworkBehaviour
{
    public int numPlayers;
    private MyNetworkManager _networkManager;

    private readonly List<PlayerInfo> _players = new List<PlayerInfo>(4);
    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;
    [SyncVar(hook = nameof(HandleDisplayRanking))] string ordenRanking = "";


    [SerializeField]public Text rankingBox;



    private void Awake()
    {
        if (_networkManager == null) _networkManager = FindObjectOfType<MyNetworkManager>();
        if (_circuitController == null) _circuitController = FindObjectOfType<CircuitController>();

        _debuggingSpheres = new GameObject[_networkManager.maxConnections];
        for (int i = 0; i < _networkManager.maxConnections; ++i)
        {
            _debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void Update()
    {
        if (_players.Count == 0){
            return; 
        }
           
   
        CmdSetRanking();
        
    }

    public void AddPlayer(PlayerInfo player)
    {
        _players.Add(player);
    }

    private class PlayerInfoComparer : Comparer<PlayerInfo>
    {
        float[] _arcLengths;

        public PlayerInfoComparer(float[] arcLengths)
        {
            _arcLengths = arcLengths;
        }

        public override int Compare(PlayerInfo x, PlayerInfo y)
        {
            if (_arcLengths[x.ID] < _arcLengths[y.ID])
                return 1;
            else return -1;
        }
    }

    public string UpdateRaceProgress()
    {

// Update car arc-lengths
        float[] arcLengths = new float[_players.Count];

        for (int i = 0; i < _players.Count; ++i)
        {
            arcLengths[i] = ComputeCarArcLength(i);
            _players[i].distanciaArco = arcLengths[i];
        }

        int size = _players.Count;
        PlayerInfo[] playerInfos = new PlayerInfo[size];



        for (int i = 0; i < size;  i++)
        {
            playerInfos[i] = _players[i];
        }

        //Reordenacion de los players segun su posicion en la carrera
        Array.Sort(playerInfos, delegate (PlayerInfo x, PlayerInfo y) { return x.distanciaArco.CompareTo(y.distanciaArco); });



        for (int i = 0; i < _players.Count; ++i)
        {
            _players[i] = playerInfos[i];
        }


        _players.Reverse();
        Debug.Log(_players.ToString());

        

        // _players.Sort(new PlayerInfoComparer(arcLengths));

        string myRaceOrder = "";
        int posActual = 1;
        foreach (var player in _players)
        {
            myRaceOrder += posActual +". " + player.GetComponent<SetupPlayer>().getName() + System.Environment.NewLine; //Cambiar nombre y número añadir br
            posActual++;
        }
          //  Debug.Log(myRaceOrder);
            return myRaceOrder;
       
    }

    float ComputeCarArcLength(int id)
    {
        if (_players[id] != null) {
            // Compute the projection of the car position to the closest circuit 
            // path segment and accumulate the arc-length along of the car along
            // the circuit.
            Vector3 carPos = this._players[id].transform.position;

            int segIdx;
            float carDist;
            Vector3 carProj;

            float minArcL =
                this._circuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

            this._debuggingSpheres[id].transform.position = carProj;

            if (this._players[id].CurrentLap == 0)
            {
                minArcL -= _circuitController.CircuitLength;
            }
            else
            {
                minArcL += _circuitController.CircuitLength *
                           (_players[id].CurrentLap - 1);
            }

            // _players[id].setDistancia(minArcL);

            minArcL += _players[id].vueltas * 10000;


            return minArcL;
        }

        return 0;
    }

    #region updateRanking

    [Server]
    public void UpdateRanking(){
        Debug.Log("Hola");
        ordenRanking = UpdateRaceProgress();
        Debug.Log(UpdateRaceProgress());
    }


    private void HandleDisplayRanking(string oldDisplayOrder, string newDisplayOrder){
        rankingBox.text = ordenRanking;
    }

    
    public void CmdSetRanking(){
        Debug.Log("CMdActivado");
        UpdateRanking();
    }

#endregion
}

