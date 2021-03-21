// defines strings used as keys in Photon Room/Player Custom Properties

namespace Com.WhiteSwan.OpheliaDigital
{
    public class KeyStrings
    {
        #region player properties

        public const string Ready = "ready";
        public const string ActivePlayer = "activeplayer"; // maybe room property too?

        public const string ChosenDeck = "chosendeck";
        public const string CardList = "cardlist";

        public const string UnityInstanceID = "unityinstanceid";

        #endregion

        #region room properties

        // draft and other properties
        public const string AllReady = "allready";

        public const string DraftType = "draftype";
        public const string Precon = "precon";
        public const string FullDraft = "fulldraft";

        public const string AvailableDecks = "availabledecks";

        // bytecodes for customtypes
        public const byte RP_Player = (byte)'L';
        public const byte RP_Card = (byte)'C';

        // gameplay state management
        public const string CardLoadComplete = "cardloadcomplete";


        #endregion

        #region ophelia properties - TODO: these should come from json 

        public const string Mattervoid = "Mattervoid";
        public const string Mechanicus = "Mechanicus";
        public const string Yucatec = "Yucatec";

        #endregion
    }
}

