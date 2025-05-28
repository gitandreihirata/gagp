using UnityEngine;

public class S_GAGP_Subtitles_Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TestLegenda() {
        FindObjectOfType<S_GAGP_Subtitles_Controller>().ShowSubtitle("Este é um teste!");
    }
}
