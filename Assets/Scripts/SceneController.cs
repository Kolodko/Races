using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public void Reboot()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
