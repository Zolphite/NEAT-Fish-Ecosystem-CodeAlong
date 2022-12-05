using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public FoodManager foodManager;
    public void SpawnSignleFood()
    {
        foodManager.SpawnSignleFood();
    }
}
