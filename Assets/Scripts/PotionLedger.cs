using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PotionLedger : MonoBehaviour
{
    public GameObject PosionOrder;
    string decryptedMessage;
    
    [HideInInspector] public string encryptionAlphabet_display;
    [HideInInspector] public string encryptionAlphabet;
    [HideInInspector] public string encryptedMessage;

    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYG";
    List<string> ingrediants = new List<string> { "WATER", "ALCOHOL", "VAPEPENLIQUID", "HERB", "PIZZA", "ZYN", "EGG", "TAXES", "VIAL" };

    // Start is called before the first frame update
    void Start()
    {
        EncryptAlphabet();

        decryptedMessage = "NOTICE" + 
            "IF INGREDIANTS X Y AND Z ARE MIXED" + 
            "YOU WILL BE SENTANCED TO DEATH";

        EncryptMessage(decryptedMessage);

    }

    private void EncryptMessage(string message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] != ' ')
            {
                char characterToFind = message[i];

                for (int j = 0; j < encryptionAlphabet.Length; j++)
                {
                    if (characterToFind == encryptionAlphabet[j])
                    {
                        Debug.Log(alphabet[j]);
                        encryptedMessage += alphabet[j];
                    }
                }
            }
        }
    }

    private void EncryptAlphabet()
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
                    usedIndex.Add(randomNumber);

                }
                count++;
            }
            count = 0;

            encryptionAlphabet += alphabet[usedIndex[i]];
            encryptionAlphabet_display += "[" + (i + 1) + "]" + alphabet[usedIndex[i]] + "\n";
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
            PosionOrder.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PosionOrder.SetActive(false);
        }
    }
}
