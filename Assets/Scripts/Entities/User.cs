using System;
using UnityEngine;

[Serializable]
public class User
{
    public int id;
    public string username,
        name,
        birth_date,
        gender_identity,
        last_login,
        email,
        full_name,
        first_name,
        last_name,
        api_key;

    public static User CreateFromJSON(string json)
    {
        return JsonUtility.FromJson<User>(json);
    }

}