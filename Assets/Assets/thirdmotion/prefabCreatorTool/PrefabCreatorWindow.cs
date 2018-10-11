using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

public class PrefabCreatorWindow : EditorWindow
{   
    private static PrefabCreatorWindow thisWindowLive;
    private Vector2 scrollPosTarget;

    #region Window Stuff
    void OnDisable()
    {
    }
    
    void OnDestroy()
    {
    }
    
    public static void show()
    {
        thisWindowLive = (PrefabCreatorWindow)EditorWindow.GetWindow(typeof(PrefabCreatorWindow));
    }
    
    public static void hide()
    {
        show();
        thisWindowLive.Close();
        thisWindowLive = null;
    }
    
    [MenuItem("Window/Prefab Creator")]
    static void startPrefabCreatorWindow()
    {
        show();
    }
    #endregion

    private GameObject baseRigObject;
    private List<GameObject> additionalItems = new List<GameObject>();

    private Dictionary<string,bool> perMeshToggleValue = new Dictionary<string,bool>();

    void OnGUI()
    {
        scrollPosTarget = EditorGUILayout.BeginScrollView(scrollPosTarget);
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Base Rig File:");
                    baseRigObject = EditorGUILayout.ObjectField(baseRigObject, typeof(GameObject), true) as GameObject;
                }
                EditorGUILayout.EndHorizontal();
                        
                drawMeshListFor(baseRigObject);
    
                //spacer 
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
    
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Number of items to add to base rig:");
                    additionalItems = drawArray(additionalItems);
                }
                EditorGUILayout.EndHorizontal();
                
                for (int i = 0; i < additionalItems.Count; i++)
                {
                    drawMeshListFor(additionalItems [i]);
                    //spacer 
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                }
    
                if (GUILayout.Button("Create Instance in Scene"))
                    createPrefab();
                GUILayout.Label("Make sure to drag this instance from Hierarchy to Project to create a prefab if you want to use this specific mix in another scene");
            }
        }
        EditorGUILayout.EndScrollView(); 
    }

    void drawMeshListFor(GameObject target)
    {
        if (target == null)
        {
            GUILayout.Label("Missing item, drag drop something into array above!");
        } else
        {
            GUILayout.Label(target.name);
            SkinnedMeshRenderer[] meshes = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer mesh in meshes)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label(mesh.name);
                    string key = target.name + "|" + mesh.name;
                    if (!perMeshToggleValue.ContainsKey(key))
                    {
                        perMeshToggleValue.Add(key, true);
                    }
                    perMeshToggleValue [key] = EditorGUILayout.Toggle(perMeshToggleValue [key]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void createPrefab()
    {
        if (baseRigObject == null)
        {
            Debug.LogError("You must drag drop a base rig !");
            return;
        }
        
        GameObject baseRigObj = Instantiate(baseRigObject) as GameObject;
        baseRigObj.name = baseRigObject.name;

        GameObject child = new GameObject("items");
        child.transform.parent = baseRigObj.transform;

        List<GameObject> itemList = new List<GameObject>();
        foreach (GameObject item in additionalItems)
        {
            if (item != null)
            {
                itemList.Add(Instantiate(item) as GameObject);
                itemList [itemList.Count - 1].name = item.name;
            }
        }

        //cache these before we add remove meshes
        SkinnedMeshRenderer[] baseRigMeshes = baseRigObj.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        //based on toggle value, either delete mesh or 
        //switch out bone references to the baserig instance
        //done move the mesh over to the baserig
        foreach (GameObject item in itemList)
        {
            SkinnedMeshRenderer[] itemMeshes = item.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int c = itemMeshes.Length-1; c > -1; c--)
            {
                string key = item.name + "|" + itemMeshes [c].name;
                if (perMeshToggleValue [key] == false)
                    DestroyImmediate(itemMeshes [c].gameObject);
                else
                {
                    Transform oldRootBone = itemMeshes [c].rootBone;
                    transferBones(itemMeshes [c], baseRigObj.transform);
                    itemMeshes [c].transform.parent = child.transform;
                    
                    Dictionary<string,Transform> newRootBonesMap = createNameToObjectMap<Transform>(baseRigObj.transform.GetComponentsInChildren<Transform>(true));
                    itemMeshes [c].rootBone = newRootBonesMap [oldRootBone.name];
                }
            }   
        }

        //delete all the item instances
        foreach (GameObject item in itemList)
        {
            DestroyImmediate(item);
        }       

        //based on toggles delete baserig meshes
        for (int i = baseRigMeshes.Length-1; i > -1; i--)
        {
            string key = baseRigObj.name + "|" + baseRigMeshes [i].name;
            if (!perMeshToggleValue.ContainsKey(key))
                Debug.LogWarning("Could not find:" + key);
            else if (perMeshToggleValue [key] == false)
                DestroyImmediate(baseRigMeshes [i].gameObject);
        }
    }

    public void transferBones(SkinnedMeshRenderer rend, Transform newRoot)
    {
        Transform[] originalBones = rend.bones;
        int numBones = originalBones.Length;
            
        Dictionary<string,Transform> newRootBonesMap = createNameToObjectMap<Transform>(newRoot.GetComponentsInChildren<Transform>(true));
            
        Transform[] outBones = new Transform[numBones];
        for (int ndx=0; ndx<numBones; ++ndx)
        {
            if (newRootBonesMap.ContainsKey(originalBones [ndx].name))
                outBones [ndx] = newRootBonesMap [originalBones [ndx].name];
            else
                Debug.LogWarning("transferBones, failed to find key (" + originalBones [ndx].name + ") in dictionary, this is probably breaking your characters!");
        }
        rend.bones = outBones;
        rend.quality = SkinQuality.Bone4; //forces the quality level which fixes some skinning issues (dont trust auto) 
    }
        
    public Dictionary<string,T> createNameToObjectMap<T>(T[] objects) where T : UnityEngine.Object
    {
        Dictionary<string,T> dict = new Dictionary<string, T>();
        foreach (T obj in objects)
            dict [obj.name] = obj;
        return dict;
    }

    private int itemCount = 0;
    private int newItemCount = 0;

    List<GameObject> drawArray(List<GameObject> list)
    {
        itemCount = list.Count;
        newItemCount = EditorGUILayout.IntField(newItemCount);
        if (GUILayout.Button("Change Additional item Size"))
        {
            if (newItemCount != itemCount)
                itemCount = newItemCount;
        } 
        
        //fix count   
        if (list.Count != itemCount)
        {
            if (itemCount > list.Count)
            {
                int dif = itemCount - list.Count;
                for (int i = 0; i < dif; i++)
                {
                    list.Add(null);
                }           
            } else if (itemCount < list.Count)
            {
                list.RemoveRange(itemCount, list.Count - itemCount);
            }
        }

        EditorGUILayout.BeginVertical();
        {
            //draw each object
            for (int i = 0; i < list.Count; i++)
            {
    
                list [i] = EditorGUILayout.ObjectField(list [i], typeof(GameObject), true) as GameObject;
    
            }
        }
        EditorGUILayout.EndVertical();
            
        return list;
    }
}

#endif