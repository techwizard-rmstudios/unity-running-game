using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private LayerMask _obstacleMask;
    [SerializeField]
    private Camera _camera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        RaycastHit hitInfo;
        if (!Input.GetMouseButtonDown(0) || !Physics.Raycast(this._camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, (int)this._obstacleMask))
            return;
        hitInfo.collider.gameObject.SetActive(false);
    }
}