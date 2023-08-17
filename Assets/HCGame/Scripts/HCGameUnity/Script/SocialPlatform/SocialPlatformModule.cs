using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace SocialPlatform
{
    public struct LoginData
    {
        public string UserID;
        public string AccessToken;
    }
    public class SocialPlatformModule
    {
        private FacebookHandler facebookHandler;
        private GoogleHandler googleHandler;

        public SocialPlatformModule()
        {
            facebookHandler = new FacebookHandler();
            googleHandler = new GoogleHandler();
        }

        //public void LoginFacebook()
        //{
        //    facebookHandler.LogIn();
        //}

        public async Task<LoginData> LoginFacebook()
        {
            return await facebookHandler.LogIn();
        }

        public async UniTask<LoginData> LoginGoogle()
        {
            return await googleHandler.Login();
        }
    }
}
