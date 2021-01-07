using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] GameObject Intro01 = null;
    [SerializeField] GameObject Intro02 = null;
    [SerializeField] GameObject Intro03 = null;
    [SerializeField] GameObject Intro04 = null;

    // Start is called before the first frame update
    void Start()
    {
        Intro01.SetActive(true);
        Intro02.SetActive(false);
        Intro03.SetActive(false);
        Intro04.SetActive(false);
    }

    public void SetIntro02()
    {
        Intro01.SetActive(false);
        Intro02.SetActive(true);
    }
    public void SetIntro03()
    {
        Intro02.SetActive(false);
        Intro03.SetActive(true);
    }
    public void SetIntro04()
    {
        Intro03.SetActive(false);
        Intro04.SetActive(true);
    }
}
