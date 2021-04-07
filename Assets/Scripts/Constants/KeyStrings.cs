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
        public const string ZoneMap = "zonemap";

        public const string UnityInstanceID = "unityinstanceid";

        public const string SceneLoaded = "sceneloaded";
        #endregion

        #region room properties
        // these must all be unique VALUES

        // draft and other properties
        public const string AllReady = "allready";

        public const string DraftType = "draftype";
        public const string Precon = "precon";
        public const string FullDraft = "fulldraft";

        public const string AvailableDecks = "availabledecks";


        // gameplay state management
        public const string ActorPrefix = "actor_";
        public const string CardIdentPrefix = "card_uid_";


        #endregion

        #region ophelia properties 
        // todo: maybe move these to their own file


        //TODO: these should come from json ?
        public const string Mattervoid = "Mattervoid";
        public const string Mechanicus = "Mechanicus";
        public const string Yucatec = "Yucatec";

        #endregion
    }
}

