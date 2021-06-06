using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PolePositionManager : NetworkBehaviour
{
    // Variables principales de PolePosition
    public int numPlayers;
    private MyNetworkManager _networkManager;
    public static double tiempo = 0;

    private readonly List<PlayerInfo> _players = new List<PlayerInfo>(4);
    private CircuitController _circuitController;
    private GameObject[] _debuggingSpheres;
    [SyncVar(hook = nameof(HandleDisplayRanking))] string ordenRanking = "";

    // Estas variables nos servirán para controlar el HUD
    [SerializeField]public Text rankingBox;
    [SerializeField]public Text tiempoBox;
    public bool flagAumentoTiempo;

    // A través de este método conseguimos el tiempo que está calculando.
    public string getTime(){
        return ("" +tiempo);
    }

    // Se inicializa a 0 el tiempo para la próxima conexión.
    public void resetTime(){
        tiempo = 0;
    }

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

    // Creamos un Update para poder controlar la suma del tiempo según va avanzando la carrera.
    private void Update(){
        if(flagAumentoTiempo){
        tiempo += Time.deltaTime;
        tiempoBox.text = ""+ Math.Round(tiempo, 3);
        }
        
    }


    // Dentro del FixedUpdate, ya que realiza comprobaciones más rápidas, iremos actualizando el ranking del HUD
    private void FixedUpdate()
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

    // Se va a limpiar la lista de jugadores para preparar una nueva partida.
    public void ClearPlayer(){
        _players.Clear();
    }

    // Elimina un jugador usando su nombre de la lista.
    public void EliminatePlayerByName(string name){
        foreach(PlayerInfo p in _players){
            if(p.GetComponent<SetupPlayer>().getName() == name){
                _players.Remove(p);
                break;
            }
        }
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

    // Se va actualizando las posiciones de los jugadores con respecto al recorrido.
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


        // Se obtienen todos los jugadores.
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

        // Se establece el string que va a servir como ranking para el HUD.
        string myRaceOrder = "";
        int posActual = 1;
        foreach (var player in _players)
        {
            myRaceOrder += posActual +". " + player.GetComponent<SetupPlayer>().getName() + System.Environment.NewLine; //Cambiar nombre y número añadir br
            posActual++;
        }
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

            minArcL += _players[id].CurrentLap * 10000;


            return minArcL;
        }

        return 0;
    }

    // Estas son las funciones encargadas de que el ranking sea actualizado de forma síncrona entre los clientes.
    #region updateRanking

    [Server]
    public void UpdateRanking(){
        ordenRanking = UpdateRaceProgress();
    }


    private void HandleDisplayRanking(string oldDisplayOrder, string newDisplayOrder){
        rankingBox.text = ordenRanking;
    }

    
    public void CmdSetRanking(){
        UpdateRanking();
    }

    public string getRanking(){
        return ordenRanking;
    }

#endregion
}

