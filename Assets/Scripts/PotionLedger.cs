using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionLedger : MonoBehaviour
{
    [HideInInspector] public string encryptionAlphabet;
    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYG";
    string encryptedMessage;

    // Start is called before the first frame update
    void Start()
    {
        List<int> usedIndex = new List<int>();

        for(int i = 0; i < alphabet.Length; i++)
        {
            //Get random number from alphabet
            int newIndex = Random.Range(1, alphabet.Length);

            //check if the index is valid
            while(usedIndex.Contains(newIndex))
            {
                newIndex = Random.Range(1, alphabet.Length);
            }

            usedIndex.Add(newIndex);

            encryptionAlphabet += "[";
            encryptionAlphabet += newIndex;
            encryptionAlphabet += "]";
            encryptionAlphabet += 'x';
            encryptionAlphabet += " ";

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
