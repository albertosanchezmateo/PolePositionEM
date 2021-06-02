using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{

    [SerializeField] public GameObject check1;
    [SerializeField] public GameObject check2; 
    [SerializeField] public GameObject check3;
    [SerializeField] public GameObject check4;

    [SerializeField] private int numCheckPoint;
    [SerializeField] private bool chocado;
    private Vector3[] posicionesChecks = new Vector3[4];
    private Vector3[] posicionesLookAt = new Vector3[4];

    [SerializeField] public bool sentidoContrario = false;
    [SerializeField]private int vueltas = 0;
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


        numCheckPoint = 4;
        chocado = false;
    }

    private void Update()
    {
        Vector3 jugador = this.GetComponent<Transform>().position;

       

        float distancia1 = Vector3.Distance(posicionesChecks[0], jugador);
        float distancia2 = Vector3.Distance(posicionesChecks[1], jugador);
        float distancia3 = Vector3.Distance(posicionesChecks[2], jugador);
        float distancia4 = Vector3.Distance(posicionesChecks[3], jugador);
        
        if (distancia1 <= 10){
            if(numCheckPoint !=4){
                sentidoContrario = true;
            }else{
                numCheckPoint = 1;
                sentidoContrario = false;
                vueltas++;
            }   
        }

        if (distancia2 <= 10)
        {
            if(numCheckPoint !=1){
                sentidoContrario = true;
            }else{
                numCheckPoint = 2;
                sentidoContrario = false;
            }  
            
        }

        if (distancia3 <= 10)
        {
            if(numCheckPoint !=2){
                sentidoContrario = true;
            }else{
                numCheckPoint = 3;
                sentidoContrario = false;
            }  
        }

        if (distancia4 <= 10)
        {
            if(numCheckPoint !=3){
                sentidoContrario = true;
            }else{
                numCheckPoint = 4;
                sentidoContrario = false;
            }  
        } 
        
       

        if (((Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.z)>90)&&Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.z)<280) ||((Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.x)>90)&&Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.x)<280))
        {
            chocado = true;
        }

        if (chocado)
        {
            this.GetComponent<Transform>().position = posicionesChecks[numCheckPoint-1];
            this.GetComponent<Transform>().LookAt(posicionesLookAt[numCheckPoint-1]);
            chocado = false;
        }
        
        Debug.Log("Rotation X:"+ Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.x)) ;
        Debug.Log("Rotation Z:" + Mathf.Abs(this.GetComponent<Transform>().localEulerAngles.z));
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