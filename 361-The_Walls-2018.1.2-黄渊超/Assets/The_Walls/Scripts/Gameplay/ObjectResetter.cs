using UnityEngine;
using OnefallGames;

public class ObjectResetter : MonoBehaviour {

    [SerializeField] private BoxController boxControl;
    private GameObject previousOb = null;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.GameState == GameState.Playing)
        {
			Debug.Log ("a");
            if (other.transform.parent != null)
            {
                GameObject parent = other.transform.parent.gameObject;
                if (parent.CompareTag("Finish"))
                {
                    if (parent != previousOb)
                    {
                        previousOb = parent;
                        PlayerController.Instance.ResetWallHoles();
                        boxControl.ScaleUp();
                        ScoreManager.Instance.AddScore(1);
                        SoundManager.Instance.PlaySound(SoundManager.Instance.scored);
                    }
                }
            }         
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent.gameObject.SetActive(false);
        PlayerController.Instance.stopRotate = false;
    }
}
