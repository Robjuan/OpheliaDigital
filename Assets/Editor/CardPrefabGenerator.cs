﻿using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Linq;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

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

        private Type[] GetAllEffectTypes()
        {
            string nameSpace = "Com.WhiteSwan.OpheliaDigital";
            var assem = Assembly.Load("Assembly-CSharp");

            return assem.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && t.Name.Contains("Effect_"))
                      .ToArray();
        }


        private void GeneratePrefabs(string targetPath)
        {
            // confirmation dialog
            if (overwrite)
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
                
                CardController newCard_CC = newCard.GetComponent<CardController>();
                newCard_CC.displayName = card.Name;
                newCard_CC.faction = faction;

                CardController.SlotType newCardSlot;

                if (System.Enum.TryParse<CardController.SlotType>(card.SlotType, out newCardSlot))
                {
                    // these two TP only
                    newCard_CC.effectText = card.Effect;

                    newCard_CC.SetBaseStats(card.Cost, card.Power, card.Initiative, card.Armour, card.Life);

                    // todo: remove these when effects are more implemented
                    newCard_CC.claimText = card.Claim;
                    newCard_CC.specialText = card.Special;
                    newCard_CC.passiveText = card.Passive;
                    newCard_CC.effectText = card.Effect;

                    // do we need to store this?
                    newCard_CC.devName = card.devName;

                    newCard_CC.slotType = newCardSlot;

                } else
                {
                    Debug.LogError("Invalid slottype, skipping");
                    errorCount += 1;
                    continue;
                }

                // check over every Effect Type defined, check their custom attribute to see if they are for this card

                foreach (Type t in GetAllEffectTypes())
                {
                    newCard.AddComponent(t);
                }
                foreach (var comp in newCard.GetComponents<CardEffectBase>())
                {
                    if (!comp.GetType().CustomAttributes.First().AttributeType.Name.Contains("064_ophelia"))
                    {
                        DestroyImmediate(comp);
                    }
                    else
                    {
                        if(comp.effectType == GamePlayConstants.EffectType.Claim && newCard_CC.claimText != "")
                        {
                            comp.fullText = newCard_CC.claimText;
                        }
                        if (comp.effectType == GamePlayConstants.EffectType.Passive && newCard_CC.passiveText != "")
                        {
                            comp.fullText = newCard_CC.passiveText;
                        }
                        if (comp.effectType == GamePlayConstants.EffectType.Special && newCard_CC.specialText != "")
                        {
                            comp.fullText = newCard_CC.specialText;
                        }
                        if (comp.effectType == GamePlayConstants.EffectType.TurningPoint && newCard_CC.effectText != "")
                        {
                            comp.fullText = newCard_CC.effectText;
                        }
                    }
                }


                // convert the gameobject to a prefab and store in the specified folder
                string localPath = targetPath + newCard.name + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(newCard, localPath);

                // add the devName to list of devnames, with prefix for loading
                devNameList.Add("Cards/"+faction+"/"+newCard.name); 

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
            TextAsset generatedAsset = new TextAsset(string.Join(",",cardList));
            AssetDatabase.CreateAsset(generatedAsset, "Assets/Resources/Cards/"+faction+"_generated_card_list.asset");

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