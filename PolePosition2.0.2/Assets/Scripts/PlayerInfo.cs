using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    // Se declaran todas las variables necesarias para el control del recorrido del jugador, ya sea colisiones o control de chequeos.
    [SerializeField] public GameObject check1;
    [SerializeField] public GameObject check2; 
    [SerializeField] public GameObject check3;
    [SerializeField] public GameObject check4;
    private bool flagPrimeraVuelta = false;
    private bool flagVuelta = false;
    public float distanciaArco;
    [SerializeField] private int numCheckPoint;
    [SerializeField] private bool chocado;
    private Vector3[] posicionesChecks = new Vector3[4];
    private Vector3[] posicionesLookAt = new Vector3[4];
    [SerializeField] public bool sentidoContrario = false;
    [SerializeField]private int numCheckRetrocesoSig; 
    [SerializeField]private float distanciaRetroceso;


    // Dentro del Awake se van a inicializar las posiciones de los checkpoints además de las orientaciones para los spawns.
    private void Awake()
    {
        check1 = GameObject.Find("CheckPoint1");
        posicionesChecks[0] = check1.transform.position;

        check2 = GameObject.Find("CheckPoint2");
        posicionesChecks[1] = check2.transform.position;

        check3 = GameObject.Find("CheckPoint3");
        posicionesChecks[2] = check3.transform.position;

        check4 = GameObject.Find("CheckPoint4");
        posicionesChecks[3] = check4.transform.position;

        posicionesLookAt[0] = GameObject.Find("LookAt1").transform.position;
        posicionesLookAt[1] = GameObject.Find("LookAt2").transform.position;
        posicionesLookAt[2] = GameObject.Find("LookAt3").transform.position;
        posicionesLookAt[3] = GameObject.Find("LookAt4").transform.position;


        numCheckPoint = 1;
        numCheckRetrocesoSig = 2;
        chocado = false;
    }

    // Se vuelve a escoger el FixerUpdate para realizar todas las comprobaciones y cambios necesarios para el recorrido.
    private void FixedUpdate()
    {
        Vector3 jugador = this.GetComponent<Transform>().position;

        float distancia1 = Vector3.Distance(posicionesChecks[0], jugador);
        float distancia2 = Vector3.Distance(posicionesChecks[1], jugador);
        float distancia3 = Vector3.Distance(posicionesChecks[2], jugador);
        float distancia4 = Vector3.Distance(posicionesChecks[3], jugador);
        
        // Por cada uno de los puntos de chequeo se va comprobando la distancia entre el jugador y se actualizan los parámetros del jugador en función a él.
        // El flag va a ser necesario para poder activar la vuelta una vez se haya pasado hacia el checkpoint 2, porque al estar en posiciones distintas, solo uno lo activaba.

        if (distancia1 <= 10){
            numCheckRetrocesoSig = 2;
            distanciaRetroceso = 10000000;
            if(numCheckPoint !=4 && !flagPrimeraVuelta){
            }else{
                numCheckPoint = 1;
                if(!sentidoContrario && flagVuelta){
                    this.gameObject.GetComponentInChildren<SetupPlayer>().CmdSetCurrentLap(this.gameObject.GetComponentInChildren<SetupPlayer>().getCurrentLap() + 1);
                    flagVuelta = false;
                }
                
            }   
        }

        if (distancia2 <= 10)
        {   
          
            numCheckRetrocesoSig = 3;
            distanciaRetroceso = 10000000;
            if(numCheckPoint !=1){
          
            }else{
                numCheckPoint = 2;
                flagPrimeraVuelta = true;
                flagVuelta = true;
            }  
            
        }

        if (distancia3 <= 10)
        {
            numCheckRetrocesoSig = 4;
            distanciaRetroceso = 10000000;
            if(numCheckPoint !=2){
           
            }else{
                numCheckPoint = 3;
            }  
        }

        if (distancia4 <= 10)
        {
            numCheckRetrocesoSig = 1;
            distanciaRetroceso = 10000000;
            if(numCheckPoint !=3){
              
            }else{
                numCheckPoint = 4;
               
            }  
        }
        if(distanciaRetroceso>Vector3.Distance(posicionesChecks[numCheckRetrocesoSig-1], jugador)){
            sentidoContrario = false;
        }else{
            sentidoContrario = true;
        }

        distanciaRetroceso = Vector3.Distance(posicionesChecks[numCheckRetrocesoSig-1], jugador);
        
       
        // Se comprueban las colisiones y se realiza el spawn.
        if (((Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.z)>90)&&Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.z)<280) ||((Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.x)>90)&&Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.x)<280))
        {
            chocado = true;
        }

        if (chocado) {
            
       
            this.GetComponent<Transform>().position = posicionesChecks[numCheckPoint-1];
            this.GetComponent<Transform>().LookAt(posicionesLookAt[numCheckPoint-1]);
            chocado = false;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        }
        
    }
    public string Name { get; set; }

    public int ID { get; set; }

    public int CurrentPosition { get; set; }

    public int CurrentLap { get; set; }

    public override string ToString()
    {
        return Name;
    }
}