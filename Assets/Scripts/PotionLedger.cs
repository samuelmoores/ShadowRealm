using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class PotionLedger : MonoBehaviour
{
    public GameObject PosionOrder;
    public GameObject EncrpyptionAlphabet_Display;
    [HideInInspector] public string encryptionAlphabet_display;
    [HideInInspector] public string alphabet_display;
    [HideInInspector] public string encryptionAlphabet;
    [HideInInspector] public string encryptedMessage;

    string decryptedMessage;
    
    List<string> ingrediants = new List<string> { "WATER", "ALCOHOL", "VAPEPENLIQUID", "HERB", "PIZZA", "ZYN", "EGG", "TAXES", "VIAL" };
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYG";


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 26; i++)
        {
            alphabet_display += "[" + (i + 1) + "]" + alphabet[i] + "\n";

        }

        //Scrample alphabet
        EncryptAlphabet();

        //get message
        decryptedMessage = "TAXES PIZZA EGG";

        EncryptMessage(decryptedMessage);


    }

    private void EncryptMessage(string message)
    {
        //loop through message
        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] != ' ')
            {
                //get first character of message
                char characterToFind = message[i];

                //loop through scrambled alphabet
                for (int j = 0; j < encryptionAlphabet.Length; j++)
                {
                    //find index of messages character in scrambled alphabet
                    if (characterToFind == encryptionAlphabet[j])
                    {
                        //take the number from that index in the regular alphabet
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
            EncrpyptionAlphabet_Display.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PosionOrder.SetActive(false);
            EncrpyptionAlphabet_Display.SetActive(true);

        }
    }
}
