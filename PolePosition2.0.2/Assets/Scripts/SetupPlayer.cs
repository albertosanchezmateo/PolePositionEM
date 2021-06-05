using System;
using Mirror;
using UnityEngine;
using Random = System.Random;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SetupPlayer : NetworkBehaviour
{
    [SyncVar] private int _id;
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))][SerializeField] private string _name;
    [SyncVar(hook = nameof(HandleDisplayColor))][SerializeField] private Color _colorCoche;

    private UIManager _uiManager;
    private MyNetworkManager _networkManager;
    private PlayerController _playerController;
    private PlayerInfo _playerInfo;
    private PolePositionManager _polePositionManager;

    private GameObject[] listaJugadores;
    [SyncVar] public bool _ready;
    private bool checkReady = false;

    private int maxVueltas = 2;

    private InputController _input;
    private Vector2 _movement;
    private readonly float _movementSpeed = 0.1f;

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        _id = NetworkServer.connections.Count - 1;
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        _playerInfo.ID = _id;
        _playerInfo.Name = "Player" + _id;
        _playerInfo.CurrentLap = 0;
        _polePositionManager.AddPlayer(_playerInfo);
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
    }

    #endregion

    private void Awake()
    {
        _playerInfo = GetComponent<PlayerInfo>();
        _playerController = GetComponent<PlayerController>();
        _networkManager = FindObjectOfType<MyNetworkManager>();
        _polePositionManager = FindObjectOfType<PolePositionManager>();
        _uiManager = FindObjectOfType<UIManager>();
        _input = new InputController();
    }

    // Control del InputController
    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
   
        if (isServer)
        {
            // listaJugadores = GameObject.FindGameObjectsWithTag("Player");
            // Debug.Log(listaJugadores.Length);
        }

        if (isLocalPlayer)
        {
            // listaJugadores = GameObject.FindGameObjectsWithTag("Player");
            // _playerController.enabled = true;
            // _playerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
            ConfigureCamera();
        }
    }

    private void Update()
    {
        // Control de la entrada de jugadores a la carrera
        if (isLocalPlayer)
        {
            listaJugadores = GameObject.FindGameObjectsWithTag("Player");
            checkReady = true;

            foreach (GameObject player in listaJugadores)
            {
                if (!player.GetComponent<SetupPlayer>()._ready)
                {
                    checkReady = false;
                }  
            }
        }

        // Control sincronizado del inicio de la carrera
        if (checkReady && listaJugadores.Length >= 4)
        {
            _playerController.enabled = true;
            _playerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
        }

        // Control del InputController
        _movement = _input.Player.Movement.ReadValue<Vector2>();

        // Control de la finalización de la carrera
        if (_playerInfo.vueltas == maxVueltas)
        {   
            _playerController.enabled = false;
            _playerController.Speed = 0;
            _playerController.CurrentRotation = 0;
            _playerController.InputAcceleration = 0;
            _playerController.InputSteering = 0;
            _playerController.InputBrake = 0;
        }
    }

    private void FixedUpdate()
    {
        // Cálculo de físicas del movimiento con el InputController
        transform.Translate(new Vector3(_movement.x, _movement.y, 0) * _movementSpeed);
    }

    void OnSpeedChangeEventHandler(float speed)
    {
        _uiManager.UpdateSpeed((int) speed * 5); // 5 for visualization purpose (km/h)
    }

    void ConfigureCamera()
    {
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraController>().m_Focus = this.gameObject;
    }


    #region Name
    [Server]

    // Comprobación del nombre de los juagdores
    public void SetName(string newName)
    {
        bool comprobacion = false;
        SetupPlayer[] players = (SetupPlayer[]) GameObject.FindObjectsOfType (typeof(SetupPlayer));

        foreach(SetupPlayer player in players)
        {
            if(player.getName()== newName)
            {
                comprobacion = true;
            }
        }

        if(!comprobacion)
        {
            _name = newName;
            Debug.Log("He conseguido cambiar de nombre");

        } else {
            Debug.Log("No he conseguido cambiar de nombre");
        }

        
    }


    private void HandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        
    }

    [Command]
    public void CmdSetName(string newName){
        SetName(newName);
    }

    public string getName(){
        return _name;
    }

    void CmdCalculateMovement(Vector2 vector)
    {
        _movement = vector;
    }
    #endregion

    #region Color
    [Server]
    public void setColor(Color newColor){
        _colorCoche = newColor;
    }

    private void HandleDisplayColor(Color oldColor, Color newColor){

    }
    
    [Command]
    public void CmdSetColor(Color newColor){
        setColor(newColor);
    }
    #endregion

    #region Ready
    [Server]
    public void setReady(bool newState)
    {
        _ready = newState;
    }

    private void HandleReady(bool oldState, bool newState)
    {

    }

    [Command]
    public void CmdSetReady(bool newState)
    {
        setReady(newState);
    }
    #endregion




}