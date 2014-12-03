using System;
using System.Net;
using System.IO; 


class Test
{
  static void Main(string[] args)
  {
     Console.WriteLine( "test .NET proxy w/ default network credentials" );

     string winProxyAddress = Environment.GetEnvironmentVariable( "WIN_PROXY" );
     if( winProxyAddress == null )
     {
        Console.WriteLine( "*** error - WIN_PROXY env varibale missing, please set e.g.:");
        Console.WriteLine( "  $ set WIN_PROXY=http://proxy.bigcorp:57416");
        Environment.Exit( 1 );  // note: 0 is OK, 1..N  is ERROR
    }

     Console.WriteLine( "  WIN_PROXY="+ winProxyAddress );

     WebProxy proxy = new WebProxy( winProxyAddress );
     proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

     WebClient client = new WebClient();
     client.Proxy = proxy;

     Console.WriteLine( "  before OpenRead" );
     StreamReader reader = new StreamReader( client.OpenRead( "http://www.orf.at" ));
     Console.WriteLine( "  after OpenRead" );

     string line = null;
     int lineno = 0;
     while( (line=reader.ReadLine()) != null ) {
       lineno++;
       if( lineno <= 10 )
         Console.WriteLine( "["+ lineno + "] " + line);     // print first couple of lines
     }
  } // fn main
} // class Test
