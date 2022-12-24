using UnityEngine;

public class CreateBlock : MonoBehaviour
{
    public GameObject[] Tetris;
    // Start is called before the first frame update
    void Start()
    {
        NewTetris();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewTetris()
    {
        Instantiate(Tetris[Random.Range(0, Tetris.Length)], transform.position, Quaternion.identity);
    }

}
