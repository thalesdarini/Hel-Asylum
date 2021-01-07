using UnityEngine;

public class CameraFinal : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadMenu()
    {
        FindObjectOfType<SceneLoader>().LoadMenu();
    }
}
