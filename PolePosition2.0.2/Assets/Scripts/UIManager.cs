using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Se establecen todas las secciones que vamos a ver en el inspector con todas sus variables, según las interfaces a crear, para facililtar su uso posteriormente.
    public bool showGUI = true;

    private MyNetworkManager m_NetworkManager;

    [Header("Pantalla Final")]
    [SerializeField] private GameObject menuFinal;
    [SerializeField] private Text puestoFinal;
    [SerializeField] private Text tiempoFinal;
    [SerializeField] private Text vueltasFinal;
    [SerializeField] private Button buttonSalirFinal;

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
    [SerializeField] private Button buttonSalir;
    [SerializeField] private Text textTiempo;

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

    private void Awake()
    {
        m_NetworkManager = FindObjectOfType<MyNetworkManager>();
    }

    // Se les da funcionalidad con delegados a los diferentes botones.
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
        buttonSalir.onClick.AddListener(() => salirMenu());
        buttonSalirFinal.onClick.AddListener(() => salirMenu());
        ActivateMainMenu();

        for(int i =0; i<buttonActivado.Length;i++){
            buttonActivado[i] = true;
        }
    }

    public void UpdateSpeed(int speed)
    {
        textSpeed.text = "Speed " + speed + " Km/h";
    }

    // Cada uno de los Activate decide que interfaz se va a ver y cuál no.
    public void ActivateMainMenu()
    {
        
        mainMenu.SetActive(true);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(false);
        menuFinal.SetActive(false);
    }

    public void ActivateInGameHUD()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(true);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(false);
        menuFinal.SetActive(false);
    }

    private void ActivateSeleccionNombre()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(true);
        menuFinal.SetActive(false);
    }

    private void ActivateCancelar()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(true);
        menuSeleccionNombre.SetActive(false);
        menuFinal.SetActive(false);
    }

    private void ActivateMenuFinal()
    {
        mainMenu.SetActive(false);
        inGameHUD.SetActive(false);
        menuCancelar.SetActive(false);
        menuSeleccionNombre.SetActive(false);
        menuFinal.SetActive(true);
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

    // Se comprueba que no pueda haber más nombres iguales y que el input no esté vacío.
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

    // Cambia el color del coche.
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

    // Sirve para poder sincronizar cuando un jugador decide cambiar algunos de los ajustes de personalización.
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

    // Permite verificar los cambios realizados en los ajustes de personalización.
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

    // Declara las nuevas vueltas para el HUD.
    public void setNewLaps(int maxVueltas){
        SetupPlayer[] players = (SetupPlayer[]) GameObject.FindObjectsOfType (typeof(SetupPlayer));
        foreach(SetupPlayer player in players){
                if(player.hasAuthority){
                   
                    textLaps.text = player.getCurrentLap() + "/" + maxVueltas;
                }
            }

        
    }

    // Permite volver a la interfaz del menú principal.
    private void salirMenu(){
        SetupPlayer[] players = (SetupPlayer[])GameObject.FindObjectsOfType(typeof(SetupPlayer));
        foreach (SetupPlayer player in players)
        {
            if (player.hasAuthority)
            {
                player.restartCamera();

                

            }
        }
    }

    // Muestra la interfaz que se encarga de dar los resultados finales de la partida.
    public void finalPartida(string tiempo){
        ActivateMenuFinal();

        SetupPlayer[] players = (SetupPlayer[]) GameObject.FindObjectsOfType (typeof(SetupPlayer));
        foreach(SetupPlayer player in players){
                if(player.hasAuthority){

                    string[] strings = GameObject.Find("@PolePositionManager").GetComponent<PolePositionManager>().getRanking().Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    foreach(string puesto in strings){
                        if(puesto.Contains(player.getName())){
                            puestoFinal.text = "" + puesto[0] ;
                        }
                    }
                   
                   tiempoFinal.text = tiempo;
                    vueltasFinal.text = "" + (player.getCurrentLap()-1);
                    
                }
            }

        


    }
}