using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class RandomModel : MonoBehaviour
    {
        void Start()
        {
            // get the total number of children
            int childCount = transform.childCount;

            // if there are no children, return
            if (childCount == 0)
                return;

            // generate a random index
            int randomIndex = Random.Range(0, childCount);

            // enable the randomly selected child and disable others
            for (int i = 0; i < childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == randomIndex);
            }
        }
    }
}
