using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Com.WhiteSwan.OpheliaDigital
{
    public class CardPrefabGenerator : EditorWindow
    {
        
        private UnityEngine.Object cardsJson;
        private GameObject cardPrefab;
        private string targetPathInput = "";
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
                targetPathInput = EditorUtility.OpenFolderPanel("Folder to store prefabs", Application.dataPath + "/Resources/Cards", "");
            }
            EditorStyles.label.wordWrap = true;

            string pathLabel = "";
            if (targetPathInput != "")
            {
                pathLabel = "Assets" + targetPathInput.Substring(Application.dataPath.Length);
            }
            EditorGUILayout.LabelField(pathLabel);

            EditorGUILayout.Space();
            overwrite = GUILayout.Toggle(overwrite, "Overwrite Existing Prefabs");

            if (GUILayout.Button("Generate Prefabs"))
            {
                GeneratePrefabs(targetPathInput);
            }
        }



        
        private void GeneratePrefabs(string targetPath)
        {
            // confirmation dialog
            if(overwrite)
            {
                bool check = EditorUtility.DisplayDialog("Overwrite Prefabs?", "Are you sure you want to overwrite existing prefabs? This is permanent.", "Proceed", "Cancel");
                if(!check)
                {
                    return;
                }
            }

            CardController.Faction faction;
            // make the path relative to the project
            if (targetPath.StartsWith(Application.dataPath))
            {
                // get faction name based on folder name
                string factionName = targetPath.Substring(Application.dataPath.Length).Substring("/Resources/Cards/".Length);

                var result = System.Enum.TryParse<CardController.Faction>(factionName, out faction);
                if(!result)
                {
                    Debug.LogError("could not parse faction folder name to CardController enum");
                    return;
                }
                Debug.Log("Matched as faction: " + faction);
                targetPath = "Assets" + targetPath.Substring(Application.dataPath.Length) + "/";
            }
            else
            {
                Debug.LogError("invalid folder selected");
                return;
            }
            
            // resources.load expects a different relative path, prep that
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
            int errorCount = 0;

            List<string> devNameList = new List<string>();

            foreach (CardDataHolder card in cardDataCollection.cards)
            {

                if(card.devName == "")
                {
                    Debug.LogWarning("empty devName, likely incomplete card, skipping");
                    errorCount += 1;
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
                    // we'll need to add these ones to the list of cards to make sure we get all
                    devNameList.Add(existingPrefab.name);

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
                newCardCardController.faction = faction;

                CharacterCardController.SlotType newCardSlot;

                if (card.SlotType == "Turning Point")
                {
                    TurningPointCardController newCardTPC = newCard.AddComponent<TurningPointCardController>();

                    newCardTPC.effectText = card.Effect;
                    newCardTPC.characterCountRequirement = card.Cost;


                } else if (System.Enum.TryParse<CharacterCardController.SlotType>(card.SlotType, out newCardSlot))
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

                    newCardCC.slotType = newCardSlot;

                } else
                {
                    Debug.LogError("Invalid slottype, skipping");
                    errorCount += 1;
                    continue;
                }


                // convert the gameobject to a prefab and store in the specified folder
                string localPath = targetPath + newCard.name + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(newCard, localPath);

                // add the devName to list of devnames
                devNameList.Add(newCard.name);

                // destory the gameobject we created in the scene
                DestroyImmediate(newCard);

                generatedCount += 1;
            }

            // save all the devnames
            SaveCardList(devNameList, faction);

            Debug.Log("Finished. Generated " + generatedCount + ", Destroyed " + destroyedCount + ", Skipped " + skippedCount + ", Errored " + errorCount );
        }

        public void SaveCardList(List<string> cardList, CardController.Faction faction)
        {
            string saveFilePath = Application.dataPath + "/Resources/Cards/" + faction + "_generated_card_list"; // + DateTime.UtcNow.ToString("yyyyMMddHHmm");

            if(File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
            FileStream fs = new FileStream(saveFilePath, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(fs, cardList);
            } 
            catch (SerializationException e)
            {
                Debug.LogError("Serialisation failed bc : " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

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