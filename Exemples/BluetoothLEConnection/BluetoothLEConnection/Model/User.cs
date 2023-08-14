using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

//using CoreML;

namespace BluetoothLEConnection.Model;

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserAddress { get; set; }
    public string UserCodePostal { get; set; }
    public string UserEmail { get; set; }


    public User()
    {
        ;
    }

    public User(int p_userId, string p_userName, string p_userAddress, string p_userCodePostal, string p_userEmail)
    {
        this.UserId = p_userId;
        this.UserName = p_userName;
        this.UserAddress = p_userAddress;
        this.UserCodePostal = p_userCodePostal;
        this.UserEmail = p_userEmail;
    }


    public override string ToString()
    {
        return $"Id : {this.UserId}" +
            $"\nNom : {this.UserName}" +
            $"\nAdresse : {this.UserAddress}" +
            $"\nCode Postal : {this.UserCodePostal}" +
            $"\nEmail : {this.UserEmail}";
    }
}


[JsonSerializable(typeof(List<Model.User>))]
internal sealed partial class UserContext : JsonSerializerContext
{

}