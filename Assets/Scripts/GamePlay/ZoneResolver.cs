using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace Com.WhiteSwan.OpheliaDigital
{
    public static class ZoneResolver
    {
        public static Dictionary<int, CardsZone.LocalZoneType> GetZoneMap(Player player)
        {
            if(player.IsLocal && GameStateManager.current.localPlayerZoneMap != null)
            {
                return GameStateManager.current.localPlayerZoneMap;
            }
            else
            {
                return ConvertZoneMapForLocal((Dictionary<int, byte>)player.CustomProperties[KeyStrings.ZoneMap]);
            }
        }

        public static Dictionary<int, CardsZone.LocalZoneType> GenerateZoneMap(int turnOrder)
        {
            // todo: we will have to redo zones a lot if >2 players are allowed
            Dictionary<int, CardsZone.LocalZoneType> zoneMap = new Dictionary<int, CardsZone.LocalZoneType>();

            zoneMap[1] = CardsZone.LocalZoneType.MyHand;
            zoneMap[2] = CardsZone.LocalZoneType.MyDeck;
            zoneMap[3] = CardsZone.LocalZoneType.MyFZ;
            zoneMap[4] = CardsZone.LocalZoneType.OppHand;
            zoneMap[5] = CardsZone.LocalZoneType.OppDeck;
            zoneMap[6] = CardsZone.LocalZoneType.OppFZ;
            zoneMap[7] = CardsZone.LocalZoneType.MyBoardLeft;
            zoneMap[8] = CardsZone.LocalZoneType.MyBoardCentre;
            zoneMap[9] = CardsZone.LocalZoneType.MyBoardRight;
            zoneMap[10] = CardsZone.LocalZoneType.OppBoardLeft;
            zoneMap[11] = CardsZone.LocalZoneType.OppBoardCentre;
            zoneMap[12] = CardsZone.LocalZoneType.OppBoardRight;
            zoneMap[13] = CardsZone.LocalZoneType.NeutralBoardLeft;
            zoneMap[14] = CardsZone.LocalZoneType.NeutralBoardRight;
            zoneMap[15] = CardsZone.LocalZoneType.RemovedFromGame;

            if (turnOrder != 0) // if we're not the first player, mirror the zone mappings
            {
                var diffZoneMap = zoneMap; // TODO: altering collection while looping over it sometimes breaks
                foreach (int key in new List<int>(diffZoneMap.Keys))
                {
                    switch (zoneMap[key])
                    {
                        case CardsZone.LocalZoneType.MyHand:
                            zoneMap[key] = CardsZone.LocalZoneType.OppHand;
                            break;
                        case CardsZone.LocalZoneType.MyDeck:
                            zoneMap[key] = CardsZone.LocalZoneType.OppDeck;
                            break;
                        case CardsZone.LocalZoneType.MyFZ:
                            zoneMap[key] = CardsZone.LocalZoneType.OppFZ;
                            break;
                        case CardsZone.LocalZoneType.OppHand:
                            zoneMap[key] = CardsZone.LocalZoneType.MyHand;
                            break;
                        case CardsZone.LocalZoneType.OppDeck:
                            zoneMap[key] = CardsZone.LocalZoneType.MyDeck;
                            break;
                        case CardsZone.LocalZoneType.OppFZ:
                            zoneMap[key] = CardsZone.LocalZoneType.MyFZ;
                            break;
                        case CardsZone.LocalZoneType.MyBoardLeft:
                            zoneMap[key] = CardsZone.LocalZoneType.OppBoardRight;
                            break;
                        case CardsZone.LocalZoneType.MyBoardCentre:
                            zoneMap[key] = CardsZone.LocalZoneType.OppBoardCentre;
                            break;
                        case CardsZone.LocalZoneType.MyBoardRight:
                            zoneMap[key] = CardsZone.LocalZoneType.OppBoardLeft;
                            break;
                        case CardsZone.LocalZoneType.OppBoardLeft:
                            zoneMap[key] = CardsZone.LocalZoneType.MyBoardRight;
                            break;
                        case CardsZone.LocalZoneType.OppBoardCentre:
                            zoneMap[key] = CardsZone.LocalZoneType.MyBoardCentre;
                            break;
                        case CardsZone.LocalZoneType.OppBoardRight:
                            zoneMap[key] = CardsZone.LocalZoneType.MyBoardLeft;
                            break;
                        case CardsZone.LocalZoneType.NeutralBoardLeft:
                            zoneMap[key] = CardsZone.LocalZoneType.NeutralBoardRight;
                            break;
                        case CardsZone.LocalZoneType.NeutralBoardRight:
                            zoneMap[key] = CardsZone.LocalZoneType.NeutralBoardLeft;
                            break;
                        case CardsZone.LocalZoneType.RemovedFromGame:
                            zoneMap[key] = CardsZone.LocalZoneType.RemovedFromGame;
                            break;

                    }

                }

            }

            return zoneMap;
        }

        public static Dictionary<int, byte> ConvertZoneMapForTransport(Dictionary<int, CardsZone.LocalZoneType> zoneMap)
        {
            Dictionary<int, byte> zoneMapForTransport = new Dictionary<int, byte>();
            foreach (int key in zoneMap.Keys)
            {
                zoneMapForTransport.Add(key, (byte)zoneMap[key]);
            }

            return zoneMapForTransport;
        }
        

        public static Dictionary<int, CardsZone.LocalZoneType> ConvertZoneMapForLocal(Dictionary<int, byte> zoneMap)
        {
            Dictionary<int, CardsZone.LocalZoneType> dictOut = new Dictionary<int, CardsZone.LocalZoneType>();
            foreach (int key in zoneMap.Keys)
            {
                dictOut.Add(key, (CardsZone.LocalZoneType)zoneMap[key]);
            }
            return dictOut;
        }
}
}