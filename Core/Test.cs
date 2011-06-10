using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialAuth.BusinessObjects;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocialAuth
{
    public class Test
    {




        public static void Main()
        {

            string profileJson = @"{""id"":""650778379"",""name"":""Deepak Aggarwal"",""first_name"":""Deepak"",""last_name"":""Aggarwal"",""link"":""http:\/\/www.facebook.com\/profile.php?id=650778379"",""education"":[{""school"":{""id"":""107927955908210"",""name"":""Cambridge""},""year"":{""id"":""116425465042670"",""name"":""1999""},""type"":""High School""}],""gender"":""male"",""timezone"":5.5,""locale"":""en_US"",""verified"":true,""updated_time"":""2010-09-04T12:07:32+0000""}";
 
            JObject j = JObject.Parse(profileJson);
            Console.WriteLine(j.SelectToken("id"));



            //List<Type> types = new List<Type>();
            //Console.WriteLine(Assembly.GetExecutingAssembly().GetTypes()
            //    .Where(x => typeof(IProvider).IsAssignableFrom(x) && x.IsClass).Count());    


            //Console.WriteLine(Utility.GetAllImplementors(typeof(IProvider)).Count());
            //List<Type> list = Utility.GetAllImplementors(typeof(IProvider));
            //list.Cast<IProvider>();

            //foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            //    Console.WriteLine("type = " + t.Name
            //        + "\t" + typeof(IProvider).IsAssignableFrom(t));


            UriBuilder ub = new UriBuilder("http://asdasd.com?a=1&b=2&c=3");
            ub.SetQueryparameter("d", "4");
            ub.SetQueryparameter("a", "34");
            Console.WriteLine(ub.ToString());
            Console.ReadKey();

        }

    }
}
