using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public List<GameObject> page1 = new List<GameObject>();
    public List<GameObject> page2 = new List<GameObject>();

    private int currentPage = 1;
    private bool transitioning = false;

    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continue(InputAction.CallbackContext context) {
        if (context.started && !this.transitioning) {
            StartCoroutine("Transition");
        }
    }

    IEnumerator Transition()
    {
        transitioning = true;

        List<GameObject> page;
        if (this.currentPage == 1) {
            page = this.page1;
        } else if (this.currentPage == 2) {
            page = this.page2;
        } else {
            yield break;
        }

        for (float ft = 1f; ft >= -0.01f; ft -= 0.1f)
        {
            foreach (GameObject obj in page) {
                TMP_Text textComp = obj.GetComponent<TMP_Text>();

                if (!textComp) continue;

                Color color = textComp.color;
                color.a = ft;

                textComp.color = color;
            }
            
            yield return new WaitForSeconds(.1f);
        }

        this.currentPage++;

        if (this.currentPage == 1) {
            page = this.page1;
        } else if (this.currentPage == 2) {
            page = this.page2;
        } else {
            this.StartGame();
            yield break;
        }

        for (float ft = 0; ft <= 1.01f; ft += 0.1f)
        {
            foreach (GameObject obj in page) {
                TMP_Text textComp = obj.GetComponent<TMP_Text>();

                if (!textComp) continue;

                Color color = textComp.color;
                color.a = ft;

                textComp.color = color;
            }

            yield return new WaitForSeconds(.1f);
        }

        transitioning = false;
    }

    private void StartGame()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("SampleScene"));
    }
}
