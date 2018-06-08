using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public GameObject lastButton;
    public GameObject nextButton;

    int currentSceneId = 1;

    Camera cameraToPreserve;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start()
    {
        cameraToPreserve = FindObjectOfType<Camera>();
        
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        currentSceneId = 1;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneId));
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            LastScene();
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            NextScene();
        }
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        //RemoveNewSceneCamera();
    }

    public void NextScene()
    {
        StartCoroutine(LoadNextScene());
    }

    public void LastScene()
    {
        StartCoroutine(LoadLastScene());
    }

    private IEnumerator LoadNextScene()
    {
        int nextSceneId = currentSceneId % (SceneManager.sceneCountInBuildSettings - 1) + 1;
        yield return SceneManager.UnloadSceneAsync(currentSceneId);

        //cameraToPreserve.gameObject.SetActive(true);
        //cameraToPreserve.transform.position = Vector3.zero;
        //cameraToPreserve.transform.rotation = Quaternion.identity;

        yield return SceneManager.LoadSceneAsync(nextSceneId, LoadSceneMode.Additive);
        currentSceneId = nextSceneId;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneId));
    }

    private IEnumerator LoadLastScene()
    {
        int lastSceneId = (currentSceneId - 1 != 0)? currentSceneId - 1 : SceneManager.sceneCountInBuildSettings - 1;
        
        yield return SceneManager.UnloadSceneAsync(currentSceneId);

        //cameraToPreserve.gameObject.SetActive(true);
        //cameraToPreserve.transform.position = Vector3.zero;
        //cameraToPreserve.transform.rotation = Quaternion.identity;

        yield return SceneManager.LoadSceneAsync(lastSceneId, LoadSceneMode.Additive);
        currentSceneId = lastSceneId;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneId));
    }

    private void RemoveNewSceneCamera()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        for (int i = 0; i < cameras.Length; ++i)
        {
            if (cameras[i] != cameraToPreserve)
            {
                MaintainCamera mc = cameras[i].GetComponent<MaintainCamera>();
                if (mc != null)
                {
                    if (mc.MaintainGameObject)
                    {
                        cameraToPreserve.gameObject.SetActive(false);
                    }
                    else if (mc.MaintainTransform)
                    {
                        cameras[i].gameObject.SetActive(false);
                        cameraToPreserve.transform.position = mc.transform.position;
                        cameraToPreserve.transform.rotation = mc.transform.rotation;
                    }

                    if (mc.NeedMoveController)
                    {
                        mc.gameObject.AddComponent<CameraController>();
                    }
                }
                else
                {
                    cameras[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
