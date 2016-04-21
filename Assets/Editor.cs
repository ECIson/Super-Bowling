using UnityEngine;
using System.Collections;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class Editor : MonoBehaviour {

    private RaycastHit hit;

    public GameObject wandImage;
    public GameObject wandEnd;

    public GameObject ARCam;
    public GameObject mainWorkspace;

    public Button selectButton;
    public Button createButton;
    public Button deleteButton;

    public Button backButton;
    public Button createBackButton;
    public Button confirmButton;

    public Button translateButton;
    public Button rotateButton;
    public Button scaleButton;
    public Button optionsButton;

    public int toolMode = 0;
    //0 is main menu
    //1 is select
    //2 is create
    //3 is delete
    //4 is object selected
    //5 is translate

    public Text title;

    public GameObject editorMenu;
    public GameObject createMenu;
    public GameObject confirmMenu;
    public GameObject transformMenu;

    public GameObject createObjectChooser;

    public Material sMat;

    public GameObject selectedObject;
    public Material[] selectedMats;

    public int objectNumber = 0;
    private ArrayList objects = new ArrayList();
    private Vector3 originalScale;

	// Use this for initialization
	void Start () {

        title.text = "Editor Menu";
        editorMenu.SetActive(true);
        confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        createMenu.SetActive(false);
        createObjectChooser.SetActive(false);
        transformMenu.SetActive(false);

        selectButton.onClick.AddListener(delegate
        {
            selectPressed(selectButton);
        });

        createButton.onClick.AddListener(delegate
        {
            createPressed(createButton);
        });

        deleteButton.onClick.AddListener(delegate
        {
            deletePressed(deleteButton);
        });

        backButton.onClick.AddListener(delegate
        {
            backPressed(backButton);
        });

        createBackButton.onClick.AddListener(delegate
        {
            backPressed(createBackButton);
        });

        confirmButton.onClick.AddListener(delegate
        {
            confirmPressed(confirmButton);
        });

        translateButton.onClick.AddListener(delegate
        {
            translatePressed(translateButton);
        });

        rotateButton.onClick.AddListener(delegate
        {
            rotatePressed(rotateButton);
        });

        scaleButton.onClick.AddListener(delegate
        {
            scalePressed(scaleButton);
        });

        optionsButton.onClick.AddListener(delegate
        {
            optionsPressed(optionsButton);
        });

	}
	
	// Update is called once per frame
	void Update () {

        if (touchedAnObject())
        {
            Ray ray;

            if (Input.touchCount > 0)
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            else
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                Debug.Log(hit.transform.name);
            }
            if (Physics.Raycast(ray, out hit) && FindParentWithTag(hit.transform.gameObject, "Obstacle") != null
                && FindParentWithName(hit.transform.gameObject, "EditorWorkspace") != null && toolMode == 1)
            {
                GameObject obstacle = FindParentWithTag(hit.transform.gameObject, "Obstacle");
                selectObject(obstacle);

                title.text = "Confirm the object to select it (" + hit.transform.gameObject + " selected)";
                confirmButton.gameObject.SetActive(true);
            }
            else if (Physics.Raycast(ray, out hit) && FindParentWithTag(hit.transform.gameObject, "Obstacle") != null 
                && FindParentWithName(hit.transform.gameObject, "ObjectChooser") != null && toolMode == 2)
            {
                GameObject obj;
                GameObject obstacle = FindParentWithTag(hit.transform.gameObject, "Obstacle");
                obj = (GameObject)Instantiate(obstacle, mainWorkspace.transform.position, Quaternion.identity);
                obj.name = obstacle.name + objectNumber;

                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.position = mainWorkspace.transform.position + new Vector3(0, 3.0f, 0);
                obj.transform.parent = mainWorkspace.transform;
                
                objectNumber++;
                Debug.Log(obj);
                objects.Add(obj);

                backPressed(createBackButton);
                selectPressed(selectButton);
                selectObject(obj);
                confirmPressed(confirmButton);
            }
            else if (Physics.Raycast(ray, out hit) && FindParentWithTag(hit.transform.gameObject, "Obstacle") != null
                && FindParentWithName(hit.transform.gameObject, "EditorWorkspace") != null && toolMode == 3)
            {
                GameObject obstacle = FindParentWithTag(hit.transform.gameObject, "Obstacle");
                selectObject(obstacle);

                title.text = "Confirm the object to delete it (" + hit.transform.gameObject + " selected)";
                confirmButton.gameObject.SetActive(true);
            }
        }

        if (toolMode == 2)
        {
            foreach (Transform child in createObjectChooser.GetComponentInChildren<Transform>())
                if (child.name != "Spotlight")
                    child.Rotate(new Vector3(0.0f, 0.5f, 0.0f));
        }
        else if (toolMode == 6) // rotate
        {
            if (wandImage.GetComponent<CustomTracker>().tracking == true)
            {
                selectedObject.transform.rotation = wandEnd.transform.rotation;
            }
        }
        else if (toolMode == 7) // scale
        {
            if (wandImage.GetComponent<CustomTracker>().tracking == true)
            {
                float dist;

                dist = Vector3.Distance(selectedObject.transform.position, wandEnd.transform.position);
                Vector3 direction = selectedObject.transform.position + wandEnd.transform.position;
                direction.Normalize();

                //Debug.Log(scaleFactor);
                if (dist < 20 && dist > 0.01)
                {
                    selectedObject.transform.localScale = (dist * 0.04f) * direction;//new Vector3(dist * dist * 0.1f, dist * dist * 0.1f, dist * dist * 0.1f);
                    selectedObject.transform.localScale = new Vector3(Mathf.Abs(selectedObject.transform.localScale.x), 
                        Mathf.Abs(selectedObject.transform.localScale.y), Mathf.Abs(selectedObject.transform.localScale.z));
                }
                else
                {
                    selectedObject.transform.localScale = originalScale;
                }
            }

        }
	
	}

    public static GameObject FindParentWithName(GameObject childObject, string name)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.name == name)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }

    public static GameObject FindParentWithTag(GameObject childObject, string tag)
    {
        Transform t = childObject.transform;
        while (t.parent != null)
        {
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }

    bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            );
    }

    bool touchedAnObject()
    {
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverGameObject(Input.GetTouch(0).fingerId)) 
            || (Input.mousePresent && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject());
    }

    void deSelect()
    {
        if (selectedObject != null)
        {
            revertObject(selectedObject, selectedMats);
            selectedObject = null;
            selectedMats = null;
        }
    }

    Material[] saveMats(GameObject p)
    {
        Transform[] c = p.GetComponentsInChildren<Transform>();
        Material[] m = new Material[c.Length];
        int i = 0;
        foreach (Transform child in c)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                m[i] = child.GetComponent<Renderer>().material;
            }
            i = i + 1;
        }
        return m;
    }

    void revertObject(GameObject p, Material[] m)
    {
        Transform[] c = p.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform child in c)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                child.GetComponent<Renderer>().material = m[i];
            }
            i = i + 1;
        }
    }

    void selectObject(GameObject p)
    {
        selectedObject = p;
        selectedMats = saveMats(p);
        Transform[] c = p.GetComponentsInChildren<Transform>();
        foreach (Transform child in c)
        {
            if (child.GetComponent<Renderer>() != null)
            {
                child.GetComponent<Renderer>().material = sMat;
            }
        }
    }

    public void backPressed(Button b)
    {
        if (toolMode == 1) // select
        { 
            deSelect();
            toolMode = 0;
            editorMenu.SetActive(true);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            title.text = "Editor Menu";
        }
        else if (toolMode == 2) // create
        {
            deSelect();
            toolMode = 0;
            editorMenu.SetActive(true);
            createMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            createObjectChooser.SetActive(false);
            title.text = "Editor Menu";
        }
        else if (toolMode == 3) // delete
        {
            deSelect();
            toolMode = 0;
            editorMenu.SetActive(true);
            createMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            createObjectChooser.SetActive(false);
            title.text = "Editor Menu";
        }
        else if (toolMode == 4) // transform
        {
            deSelect();
            toolMode = 0;
            editorMenu.SetActive(true);
            createMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            createObjectChooser.SetActive(false);
            transformMenu.SetActive(false);
            title.text = "Editor Menu";
        }
    }

    public void confirmPressed(Button b)
    {
        if (toolMode == 1) // select
        {
            toolMode = 4;
            editorMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(false);
            transformMenu.SetActive(true);

            if (selectedObject.transform.Find("UI") != null)
            {
                optionsButton.gameObject.SetActive(true);
            }
            else
            {
                optionsButton.gameObject.SetActive(false);
            }
        }
        else if (toolMode == 3) // delete
        {
            objects.Remove(selectedObject);
            Destroy(selectedObject);
            backPressed(backButton);
        }
        else if (toolMode == 5) // translate
        {
            selectedObject.transform.parent = mainWorkspace.transform;
            toolMode = 4;
            editorMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(false);
            transformMenu.SetActive(true);
        }
        else if (toolMode == 6) // rotate
        {
            toolMode = 4;
            editorMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(false);
            transformMenu.SetActive(true);
        }
        else if (toolMode == 7) // scale
        {
            toolMode = 4;
            editorMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(false);
            transformMenu.SetActive(true);
        }
        else if (toolMode == 8) // options
        {
            toolMode = 4;
            editorMenu.SetActive(false);
            //confirmMenu.SetActive(false);
            backButton.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(false);
            transformMenu.SetActive(true);

            selectedObject.SendMessage("hideUI");
        }
    }

    public void selectPressed(Button b)
    {
        toolMode = 1;
        editorMenu.SetActive(false);
        //confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        title.text = "Touch an object to select it";
    }

    public void deletePressed(Button b)
    {
        toolMode = 3;
        editorMenu.SetActive(false);
        //confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        title.text = "Touch an object to delete it";
    }

    public void createPressed(Button b)
    {
        toolMode = 2;
        editorMenu.SetActive(false);
        createMenu.SetActive(true);
        createObjectChooser.SetActive(true);
        title.text = "Touch an object to create it";
    }

    public void translatePressed(Button b)
    {
        toolMode = 5;
        transformMenu.SetActive(false);
        //confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        selectedObject.transform.parent = ARCam.transform;
    }

    public void rotatePressed(Button b)
    {
        toolMode = 6;
        transformMenu.SetActive(false);
        //confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
    }

    public void scalePressed(Button b)
    {
        toolMode = 7;
        transformMenu.SetActive(false);
        //confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        originalScale = selectedObject.transform.localScale;
    }

    public void optionsPressed(Button b)
    {
        toolMode = 8;
        transformMenu.SetActive(false);
        //confirmMenu.SetActive(true);
        backButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);

        selectedObject.SendMessage("showUI");
    }
}
