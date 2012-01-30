using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.IdentityModel.Claims;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

public class WingtipClaimTypes
{
    // System.IdentityModel.Claims.ClaimTypes.Email;
    public static string EmailAddress =
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    public static string Authentication = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication";
    public const string DateOfBirth = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/dateofbirth";
    public const string Gender = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/gender";
    public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
    public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    public const string Upn = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
    public const string Uri = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri";
    public const string DefaultName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/displayname";
    
}

public class UserInfo
{
    // The email address is used as the UserID.
    // Every user has two claims: authentication and email address. 
    // SharePoint will pick up the email claim and treat it as identity.
    private static string[] userDB = 
       {
       "user1@wingtip.com:Authentication:Engineer", 
        "user1@wingtip.com:Email:user1@wingtip.com",
        "user1@wingtip.com:Role:facebook",
        "user2@wingtip.com:Authentication:Manager",
        "user2@wingtip.com:Email:user2@wingtip.com",
        "user2@wingtip.com:Role:socialauth",
        "user3@wingtip.com:Authentication:CEO",
        "user3@wingtip.com:Email:user3@wingtip.com",
        "user3@wingtip.com:Role:socialauth"
       };

    // Manually construct a list of users. In a production environment,
    // you should look up a directory service or database to retrieve 
    // the user information.
    public static List<string> GetAllUsers()
    {
        List<string> allUsers = new List<string>();
        //Adding forms-based users.
        allUsers.Add("user1@wingtip.com");
        allUsers.Add("user2@wingtip.com");
        allUsers.Add("user3@wingtip.com");
        return allUsers;
    }

    public static bool AuthenticateUser(string username, string password)
    {
        // Add your authentication logic here.
        return true;
    }

    /// <summary>
    /// A real implementation should look up a directory service or database 
    /// to retrieve a user's claim. The code below is used 
    /// only for demonstration purposes.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public static List<Claim> GetClaimsForUser(string username)
    {
        List<Claim> userClaims = new List<Claim>();
        foreach (string userInfo in userDB)
        {
            string[] claims = userInfo.Split(new string[] { ":" },
            StringSplitOptions.RemoveEmptyEntries);
            if (username == claims[0])
            {
                userClaims.Add(new Claim(GetClaimTypeForRole(claims[1]), claims[2],
                Microsoft.IdentityModel.Claims.ClaimValueTypes.String));
            }
        }

        return userClaims;
    }



    public static List<Claim> GetClaimsForSocialAuthUser(UserProfile profile)
    {
        List<Claim> userClaims = new List<Claim>();
        userClaims.Add(new Claim(WingtipClaimTypes.Authentication, profile.Provider.ToString()));
        userClaims.Add(new Claim(WingtipClaimTypes.EmailAddress, string.IsNullOrEmpty(profile.Email) ? "emailNotKnown" : profile.Email));
        userClaims.Add(new Claim(WingtipClaimTypes.Role, "socialauth"));
        userClaims.Add(new Claim(WingtipClaimTypes.DateOfBirth, string.IsNullOrEmpty(profile.DateOfBirth) ? "" : profile.DateOfBirth.ToString()));
        userClaims.Add(new Claim(WingtipClaimTypes.Upn, profile.ID.ToString()));
        userClaims.Add(new Claim(WingtipClaimTypes.Uri, profile.ProfilePictureURL));
        userClaims.Add(new Claim(WingtipClaimTypes.Gender, profile.Gender));
        userClaims.Add(new Claim(WingtipClaimTypes.Name, profile.FullName));
        userClaims.Add(new Claim(WingtipClaimTypes.DefaultName, profile.FullName));

        return userClaims;
    }


    public static string GetClaimTypeForRole(string roleName)
    {
        if (roleName.Equals("Authentication", StringComparison.OrdinalIgnoreCase))
            return WingtipClaimTypes.Authentication;
        else if (roleName.Equals("Email", StringComparison.OrdinalIgnoreCase))
            return WingtipClaimTypes.EmailAddress;
        else if (roleName.Equals("Role", StringComparison.OrdinalIgnoreCase))
            return WingtipClaimTypes.Role;
        
        else
            throw new Exception("Claim Type not found!");
    }



}

