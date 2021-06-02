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

        numCheckPoint = 1;
        chocado = false;
    }

    private void Update()
    {
        Vector3 jugador = this.GetComponent<Transform>().position;

        Debug.Log(jugador);

        float distancia1 = Vector3.Distance(posicionesChecks[0], jugador);
        float distancia2 = Vector3.Distance(posicionesChecks[1], jugador);
        float distancia3 = Vector3.Distance(posicionesChecks[2], jugador);
        float distancia4 = Vector3.Distance(posicionesChecks[3], jugador);

        if (distancia1 <= 10)
        {
            numCheckPoint = 1;
        }

        if (distancia2 <= 10)
        {
            numCheckPoint = 2;
        }

        if (distancia3 <= 10)
        {
            numCheckPoint = 3;
        }

        if (distancia4 <= 10)
        {
            numCheckPoint = 4;
        }

        if (Mathf.Abs(this.GetComponent<Transform>().rotation.z) >= 180)
        {
            chocado = true;
        }

        if (chocado)
        {
            this.GetComponent<Transform>().position = posicionesChecks[numCheckPoint-1];
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