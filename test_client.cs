using System;
using System.Net;
using System.IO; 


class TestClient
{
  static void Main(string[] args)
  {
     Console.WriteLine( "test .NET proxy w/ default network credentials" );

     WebProxy proxy = new WebProxy( "http://proxy.bigcorp:57416" );
     proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

     WebClient client = new WebClient();
     client.Proxy = proxy;

     Console.WriteLine( "  before OpenRead" );
     StreamReader reader = new StreamReader( client.OpenRead( "http://www.orf.at" ));
     Console.WriteLine( "  after OpenRead" );
     
     string str = null;
     while( (str=reader.ReadLine())!= null )
       Console.WriteLine(str);
  }
}

