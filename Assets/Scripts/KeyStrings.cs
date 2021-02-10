﻿// defines strings used as keys in Photon Room/Player Custom Properties

namespace Com.WhiteSwan.OpheliaDigital
{
    public class KeyStrings
    {
        #region player properties

        public const string Ready = "ready";
        public const string TurnOrder = "turnorder";
        public const string ActivePlayer = "activeplayer"; // maybe room property too?

        public const string ChosenDeck = "chosendeck";

        #endregion

        #region room properties

        public const string AllReady = "allready";

        public const string DraftType = "draftype";
        public const string Precon = "precon";
        public const string FullDraft = "fulldraft";

        public const string AvailableDecks = "availabledecks";

        #endregion

        #region ophelia properties - consider moving out?

        public const string Mattervoid = "Mattervoid";
        public const string Mechanicus = "Mechanicus";
        public const string Yucatec = "Yucatec";

        #endregion
    }
}

