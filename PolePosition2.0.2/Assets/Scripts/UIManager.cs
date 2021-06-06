using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public bool showGUI = true;

    private MyNetworkManager m_NetworkManager;

    [Header("Main Menu")] [SerializeField] private GameObject mainMenu;
    [SerializeField] private Button buttonHost;
    [SerializeField] private Button buttonClient;
    [SerializeField] private Button buttonServer;
    [SerializeField] private InputField inputFieldIP;

    [Header("In-Game HUD")] [SerializeField]
    private GameObject inGameHUD;

    [SerializeField] private Text textSpeed;
    [SerializeField] private Text textLaps;
    [SerializeField] private Text textPosition;

    [Header("Selección De Nombre")] 

    [SerializeField] private GameObject menuSeleccionNombre;
    [SerializeField] private InputField inputFieldNombreJugador;
    [SerializeField] private Button buttonAceptarNombre;
    [SerializeField] private Button buttoncolor1;
    [SerializeField] private Button buttoncolor2;
    [SerializeField] private Button buttoncolor3;
    [SerializeField] private Button buttoncolor4;
    public bool [] buttonActivado = new bool [4];


    [Header("Cancelar preparación")]
    [SerializeField] private GameObject menuCancelar;
    [SerializeField] private Button buttonCancelarReady;

    [Header("Materiales")]
    [SerializeField] private Material material1;


    private void Awake()
    {
        m_NetworkManager = FindObjectOfType<MyNetworkManager>();
    }

    private void Start()
    {
        buttonHost.onClick.AddListener(() => StartHost());
        buttonClient.onClick.AddListener(() => StartClient());
        buttonServer.onClick.AddListener(() => StartServer());
        buttonAceptarNombre.onClick.AddListener(() => comprobarNombre());
        buttoncolor1.onClick.AddListener(() => cambiarColor(1));
        buttoncolor2.onClick.AddListener(() => cambiarColor(2));
        buttoncolor3.onClick.AddListener(() => cambiarColor(3));
        buttoncolor4.onClick.AddListener(() => cambiarColor(4));
        buttonCancelarReady.onClick.AddListener(() => cancelarReady());
        ActivateMainMenu();

        for(int i =0; i<buttonActivado.Length;i++){
            buttonActivado[i] = true;
        }
    }

    public void UpdateSpeed(int speed)
    {
        textSpeed.text = "Speed " + speed + " Km/h";
    }

    public void ActivateMainMenu()
    {
        
        mainMenu.SetActive(true);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(false);
    }

    public void ActivateInGameHUD()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(true);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(false);
    }

    private void ActivateSeleccionNombre()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(true);
    }

    private void ActivateCancelar()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(true);
        menuSeleccionNombre.SetActive(false);
    }


    private void StartHost()
    {
        m_NetworkManager.StartHost();
        ActivateSeleccionNombre();
    }

    private void StartClient()
    {
        m_NetworkManager.StartClient();
        m_NetworkManager.networkAddress = inputFieldIP.text;
        ActivateSeleccionNombre();
    }

    private void StartServer()
    {
        m_NetworkManager.StartServer();
        ActivateInGameHUD();
    }

    private void comprobarNombre(){
        if(inputFieldNombreJugador.text != ""){
            SetupPlayer[] players = (SetupPlayer[]) GameObject.FindObjectsOfType (typeof(SetupPlayer));
            foreach(SetupPlayer player in players){
                if(player.hasAuthority){
                    player.CmdSetName(inputFieldNombreJugador.text);
                    entrarReady();
                    ActivateCancelar();
                }
            }
        }
    }

    private void cambiarColor(int color){
        if(buttonActivado[color-1]){
            Color colorNuevo = new Color(0f,0f,0f);
            switch(color){
                case 1:
                    colorNuevo = new Color(255f,0f, 0f);
                    break;
                case 2:
                    colorNuevo = new Color(255f,227f,0f);
                    break;
                case 3:
                    colorNuevo = new Color(0f,255f,0f);
                    break;
                case 4:
                    colorNuevo = new Color(0f,0f,255f);
                    break;
            }
              SetupPlayer[] players = (SetupPlayer[]) GameObject.FindObjectsOfType (typeof(SetupPlayer));
        foreach(SetupPlayer player in players){
                if(player.hasAuthority){
                    player.CmdSetColor(colorNuevo);
                    
                }
            }
        }
      
    }

    private void cancelarReady()
    {
        SetupPlayer[] players = (SetupPlayer[])GameObject.FindObjectsOfType(typeof(SetupPlayer));
        foreach (SetupPlayer player in players)
        {
            if (player.hasAuthority)
            {
                player.CmdSetReady(false);

            }
        }

        ActivateSeleccionNombre();
    }

    private void entrarReady()
    {
        SetupPlayer[] players = (SetupPlayer[])GameObject.FindObjectsOfType(typeof(SetupPlayer));
        foreach (SetupPlayer player in players)
        {
            if (player.hasAuthority)
            {
                player.CmdSetReady(true);

            }
        }
    }

    public void setNewLaps(int maxVueltas){
        SetupPlayer[] players = (SetupPlayer[]) GameObject.FindObjectsOfType (typeof(SetupPlayer));
        foreach(SetupPlayer player in players){
                if(player.hasAuthority){
                   
                    textLaps.text = player.getCurrentLap() + "/" + maxVueltas;
                }
            }

        
    }
}