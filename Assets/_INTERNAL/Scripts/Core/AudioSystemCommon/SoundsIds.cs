namespace Core.AudioSystemCommon
{
    public static class SoundsIds
    {
        private static string _clickSound;
        private static string _sBuy;
        private static string _fBuy;
        private static string _mainTheme;

        public static string MainTheme => _mainTheme;
        public static string SBuy => _sBuy;
        public static string FBuy => _fBuy;
        public static string ClickSound => _clickSound;

        public static void SetNewIdByType(string id, SoundType type)
        {
            switch (type)
            {
                case SoundType.Click:
                    _clickSound = id;
                    break;
                case SoundType.S_Buy:
                    _sBuy = id;
                    break;
                case SoundType.F_Buy:
                    _fBuy = id;
                    break;
                case SoundType.Main_Music:
                    _mainTheme = id;
                    break;
                default:
                    throw new System.ArgumentNullException("Invalid sound type", nameof(type));
            }
        }
    }
}