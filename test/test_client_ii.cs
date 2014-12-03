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

     Console.WriteLine( "  WIN_PROXY=" + winProxyAddress );

     WebProxy proxy = new WebProxy( winProxyAddress );
     proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

     HttpWebRequest req  = (HttpWebRequest) WebRequest.Create( "http://www.derstandard.at" );
     Console.WriteLine( "  req.RequestUri: " + req.RequestUri );

     req.Proxy = proxy;

     Console.WriteLine( "  before GetResponse()" );
     HttpWebResponse res = (HttpWebResponse) req.GetResponse();
     Console.WriteLine( "  after GetResponse()" );

     Console.WriteLine( "  res.ResponseUri: " + res.ResponseUri );   //check: get "updated" for redirects ???
     Console.WriteLine( "  res.StatusCode: " + (int)res.StatusCode + " - " + res.StatusCode );
     Console.WriteLine( "  res.StatusDescription: " + res.StatusDescription );

     Console.WriteLine( "  res.ContentType:" + res.ContentType );
     Console.WriteLine( "  res.ContentLength: " + res.ContentLength );
     Console.WriteLine( "  res.Server: " + res.Server );
     Console.WriteLine( "  res.LastModified: " + res.LastModified );

     // print all (response) headers
     for( int i=0; i < res.Headers.Count; ++i)  
       Console.WriteLine("  ["+(i+1)+"] " + res.Headers.Keys[i] + ": " + res.Headers[i] );

     StreamReader reader = new StreamReader( res.GetResponseStream() ); 
     string line = null;
     int lineno = 0;
     while( (line=reader.ReadLine()) != null ) {
       lineno++;
       if( lineno <= 10 )
         Console.WriteLine( "["+ lineno + "] " + line);     // print first couple of lines
     }
  } // fn main
} // class Test

