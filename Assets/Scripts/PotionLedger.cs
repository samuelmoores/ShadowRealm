using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PotionLedger : MonoBehaviour
{
    [HideInInspector] public string encryptionAlphabet;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYG";
    string encryptedMessage;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        List<int> usedIndex = new List<int>();
        bool numberValid = false;
        int count = 0;

        for (int i = 0; i < alphabet.Length; i++)
        {
            while (!numberValid && count < 1000)
            {
                //Get random number from alphabet
                int randomNumber = Random.Range(0, alphabet.Length);

                numberValid = !usedIndex.Contains(randomNumber);

                if (numberValid)
                {
                    Debug.Log(randomNumber + ": number valid");
                    usedIndex.Add(randomNumber);

                }
                else
                {
                    Debug.Log(randomNumber + ": not vaid");
                }
                count++;
            }
            count = 0;

            encryptionAlphabet += "[" + (i + 1) + "]" + alphabet[usedIndex[i]] + "\n";
            numberValid = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Player found ledger!");
        }
    }
}
