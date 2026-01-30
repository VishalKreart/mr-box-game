public static class AdConfig
{
    public static string AppKey => GetAppKey();
    public static string BannerAdUnitId => GetBannerAdUnitId();
    public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
    public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

    static string GetAppKey()
    {
        #if UNITY_ANDROID
            return "250c25015";
#elif UNITY_IPHONE
            return "250c3e2cd";
#else
            return "unexpected_platform";
#endif
    }

    static string GetBannerAdUnitId()
    {
        #if UNITY_ANDROID
            return "thnfvcsog13bhn08";
        #elif UNITY_IPHONE
            return "iep3rxsyp9na3rw8";
        #else
            return "unexpected_platform";
        #endif
    }
    static string GetInterstitialAdUnitId()
    {
        #if UNITY_ANDROID
            return "ze9bwovogn1rtbov";
#elif UNITY_IPHONE
            return "y6r8ifsf4hov42k2";
#else
            return "unexpected_platform";
#endif
    }

    static string GetRewardedVideoAdUnitId()
    {
        #if UNITY_ANDROID
            return "ld43iaeoqz1n0ijq";
#elif UNITY_IPHONE
            return "zwigalwmtaxanm9d";
#else
            return "unexpected_platform";
#endif
    }
}
