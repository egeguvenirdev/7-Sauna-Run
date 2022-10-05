var functions = 
{
	//**BERMUDA_WEBSDK_START**
  SDK_Init: function(gameKey) 
  {
    SendMessage("WebAdManager", "ActivateAdPreview");
  },
  
  SDK_PreloadAd: function() 
  {
    SendMessage("ExternalAdManager", "PreloadRewardedVideoCallback",1);
  },

  SDK_ShowAd: function(adType) 
  {
    SendMessage("ExternalAdManager", "RewardedVideoSuccessCallback");
  }
	//**BERMUDA_WEBSDK_END**
};

mergeInto(LibraryManager.library, functions);