using ModernDev.InTouch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSTimetable
{
    class VKClass
    {

        static async void VKInit()
        {
            var clientId = 12345; // API client Id
            var clientSecret = "super_secret"; // API client secret

            var client = new InTouch("");
            access_token = 

            client.access_token// Authorization works only in Windows (and WinPhone) Store Apps
            // otherwise you'll need to set received "access_token" manually
            // using SetSessionData method.

            // Gets a list of a user's friends.
            var friends = await client.Friends.Get();

            if (friends.IsError == false)
            {
                ShowFriendsList(friends.Data.Items.Where(friend => friend.Online));
            }
        }
       
    }
}
