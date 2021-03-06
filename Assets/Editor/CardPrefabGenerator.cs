using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class CardPrefabGenerator : EditorWindow
    {
        
        private Object cardsJson;
        private GameObject cardPrefab;
        private string targetPath = "";
        private bool overwrite = false;

        [MenuItem("Window/Ophelia Card Prefab Generator")]
        public static void ShowWindow()
        {
            GetWindow<CardPrefabGenerator>("Ophelia Card Prefab Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Ophelia Card Prefab Generator");

            cardPrefab = (GameObject)EditorGUILayout.ObjectField("Card Prefab", cardPrefab, typeof(GameObject), false);

            //cardsJson = EditorGUILayout.ObjectField("JSON Object", cardsJson, typeof(Object), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("Select Target Folder (with cards json)"))
            {
                targetPath = EditorUtility.OpenFolderPanel("Folder to store prefabs", "", "");
                // make the path relative to the project
                if(targetPath.StartsWith(Application.dataPath))
                {
                    targetPath = "Assets" + targetPath.Substring(Application.dataPath.Length) + '/';
                }
            }
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField(targetPath);


            EditorGUILayout.Space();
            overwrite = GUILayout.Toggle(overwrite, "Overwrite Existing Prefabs");

            if (GUILayout.Button("Generate Prefabs"))
            {
                GeneratePrefabs();
            }
        }



        
        private void GeneratePrefabs()
        {

            if(overwrite)
            {
                bool check = EditorUtility.DisplayDialog("Overwrite Prefabs?", "Are you sure you want to overwrite existing prefabs? This is permanent.", "Proceed", "Cancel");
                if(!check)
                {
                    return;
                }
            }

            string assetPathPrefix = "Assets/Resources/";

            string resourcesLoadPath = targetPath;
            if (resourcesLoadPath.StartsWith(assetPathPrefix))
            {
                resourcesLoadPath = resourcesLoadPath.Substring(assetPathPrefix.Length);
            }

            cardsJson = Resources.Load(resourcesLoadPath + "cards");
            if (!cardsJson)
            {
                Debug.LogError("cards json was not found in the folder");
                return;
            }

            if(!cardsJson || targetPath == "" || !cardPrefab)
            {
                Debug.LogError("not the right stuff supplied");
                return;
            }

            // save data as per CardDataHolder
            CardDataCollection cardDataCollection = JsonUtility.FromJson<CardDataCollection>(cardsJson.ToString());
            int generatedCount = 0;
            int destroyedCount = 0;
            int skippedCount = 0;
            foreach (CardDataHolder card in cardDataCollection.cards)
            {

                if(card.devName == "")
                {
                    Debug.LogWarning("empty devName, likely incomplete card, skipping");
                    skippedCount += 1;
                    continue;
                }

                // check to see if prefab exists, handle as specified
                GameObject existingPrefab = null;
                bool prefabExists = true;

                existingPrefab = (GameObject)Resources.Load(resourcesLoadPath + card.devName);
                if (existingPrefab == null)
                {
                    prefabExists = false;
                }
                
                if (!overwrite && prefabExists)
                {
                    //Debug.LogWarning("Prefab already exists, not overwriting, skipping");
                    skippedCount += 1;
                    continue;
                } else if (overwrite && prefabExists)
                {
                    DestroyImmediate(existingPrefab, true); 
                    destroyedCount += 1;
                } 

                // instantiate the CardBase prefab - this will bring all 3D stuff etc with it
                // add components and set properties as required

                GameObject newCard = (GameObject)PrefabUtility.InstantiatePrefab(cardPrefab);
                newCard.name = card.devName;

                CardController newCardCardController = newCard.GetComponent<CardController>();
                newCardCardController.displayName = card.Name;

                if (card.SlotType == "Turning Point")
                {
                    TurningPointCardController newCardTPC = newCard.AddComponent<TurningPointCardController>();

                    newCardTPC.effectText = card.Effect;
                    newCardTPC.characterCountRequirement = card.Cost;


                } else if (card.SlotType == "Historic" || card.SlotType == "Unsung")
                {
                    CharacterCardController newCardCC = newCard.AddComponent<CharacterCardController>();

                    newCardCC.baseLevel = card.Cost;
                    newCardCC.basePower = card.Power;
                    newCardCC.baseInitiative = card.Initiative;
                    newCardCC.baseArmour = card.Armour;
                    newCardCC.baseLife = card.Life;

                    newCardCC.claimText = card.Claim;
                    newCardCC.specialText = card.Special;
                    newCardCC.passiveText = card.Passive;

                    // do we need to store this?
                    newCardCC.devName = card.devName;

                    if (card.SlotType == "Historic")
                    {
                        newCardCC.slotType = CharacterCardController.SlotType.Historic;
                    } else
                    {
                        newCardCC.slotType = CharacterCardController.SlotType.Unsung;
                    }

                } else
                {
                    Debug.LogError("Invalid slottype, skipping");
                    skippedCount += 1; // mb report errors separately
                    continue;
                }


                // convert the gameobject to a prefab and store in the specified folder
                string localPath = targetPath + newCard.name + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(newCard, localPath);

                // destory the gameobject we created in the scene
                DestroyImmediate(newCard);

                generatedCount += 1;

            }

            Debug.Log("Finished. Generated " + generatedCount + ", Destroyed " + destroyedCount + ", Skipped " + skippedCount );
        }


    }


    // little classes to help unity's terrible json loader
    [System.Serializable]

    public class CardDataHolder
    {
        public string Name;
        public int Armour;
        public string Claim;
        public int Cost;
        public string Effect;
        public int Initiative;
        public int Life;
        public string Passive;
        public string SlotType;
        public string Special;
        public string devName; // in form "xxx_name" where xxx is unique number
        public int Power;
    }

    [System.Serializable]
    public class CardDataCollection
    {
        public CardDataHolder[] cards;
    }


}