using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using Vdopia;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
#if UNITY_IOS
        AuthenticateGameCenter()
#endif
    }

    public void AuthenticateGameCenter()
    {
        Social.localUser.Authenticate(success =>
        {
            if (success)
                print("Authentication Successful");
            else
                print("Authentication Failed");
        });
    }
}


