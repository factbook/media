using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;


class Server
{
  private TcpListener _listener;

  private string      _winProxyAddress;
  private WebProxy    _winProxy;

  public Server( int port )
  {
    _winProxyAddress = Environment.GetEnvironmentVariable( "WIN_PROXY" );
    _winProxy = new WebProxy( _winProxyAddress );
    _winProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

    // NOTE: loopback is equivalent to 127.0.0.1 in dotted-quad notation
    // NOTE: use loopback - avoid firewall-popup (only local machine can connect to loopback)
    _listener = new TcpListener( IPAddress.Loopback, port );   // was: IPAddress.Any
  }

private void ListenAndServe()
{
  _listener.Start();  // Starts listening for incoming connection requests
  while( true )
  {
    //blocks until a client has connected to the server
    Console.WriteLine( "  wait for client connection - blocking" );
    TcpClient client = _listener.AcceptTcpClient();
    Console.WriteLine( "  accept client connection; new request - lets go" );

    //create a thread to handle communication 
    //with connected client
    //  Thread clientThread = new Thread( new ParameterizedThreadStart(HandleClientComm));
    // clientThread.Start( client );
    HandleClientComm( client );
  }
}


private void HandleClientComm(object obj)
{
  Console.WriteLine( "begin handle client request" );

  TcpClient client = (TcpClient)obj;
  NetworkStream stream = client.GetStream();

  StreamReader r = new StreamReader( stream );

  String http_req_method    = null;
  String http_req_path      = null;
  String http_req_protocol  = null;
  
  Hashtable http_req_headers = new Hashtable();

  int i=0;
  String line = null;
  while(( line = r.ReadLine()) != null ) {
    i+=1;
    Console.WriteLine( "["+i+"] " + line );
    
    if( i==1 ) {
      // request line (first line) split in three parts
      String[] values   = line.Split( ' ' );
      http_req_method   = values[0];
      http_req_path     = values[1];
      http_req_protocol = values[2];
    }
    if( line == "" ) {
        Console.WriteLine( "empty line in http request - break");
        break;
    }
    if( i != 1 ) {
      // assume HTTP header
      int pos = line.IndexOf(':');
      if( pos != -1 )
      {
        String key   = line.Substring( 0, pos );
        String value = line.Substring( pos+2 );  // NOTE: skip : and leading space
        Console.WriteLine( "key>>" + key + "<<, value>>" + value + "<<" );
        http_req_headers[ key ] = value;
      }
    }
  }

  Console.WriteLine( "after read lines" );
  Console.WriteLine( "   |>" + http_req_method + "<|>" + http_req_path + "<|>" + http_req_protocol + "<|" );
  Console.WriteLine( "   |>" + http_req_headers["Host"] + "<|" );

  String url = null;
  // note: req_path may include/start with http://
  if( http_req_path.StartsWith( "http://" ) == true )
    url = http_req_path;
  else {
    url = http_req_headers["Host"] + http_req_path;
  }
  
  Console.WriteLine( "   url |>" + url + "<|" );

  Console.WriteLine( "before fetch response" );
  fetchResponse( url, stream );
  Console.WriteLine( "after fetch response" );

  // Console.WriteLine( "before send response" );
  // sendResponse( stream );
  // Console.WriteLine( "after send response" );

  client.Close();

  Console.WriteLine( "end handle client request" );
}

private void fetchResponse( String url, NetworkStream stream )
{
    WebClient client = new WebClient();
    client.Proxy = _winProxy;
     
    // Download data.
    Console.WriteLine( "before download data" );
    byte[] data = client.DownloadData( url );
    Console.WriteLine( "after download data" );

     // Get response header.
    string contentType   = client.ResponseHeaders["Content-Type"];
    string contentLength = client.ResponseHeaders["Content-Length"];
    Console.WriteLine( " content-type: |>" + contentType + "<|" );
    Console.WriteLine( " content-length: |>" + contentLength + "<|" );

    // print all (response) headers
    for( int i=0; i < client.ResponseHeaders.Count; ++i)  
      Console.WriteLine("  ["+(i+1)+"] " + client.ResponseHeaders.Keys[i] + ": " + client.ResponseHeaders[i] );

     // todo/fix:  use http status code
     //   check if 200 etc.

     Console.WriteLine( "begin send response" );
     Console.WriteLine( "begin send response-head" );
     var t = new StreamWriter(stream);
        t.WriteLine( "HTTP/1.0 200 OK" );
        t.WriteLine( "Content-Type: "+ contentType );
        t.WriteLine( "Server: proxy-win/0.1" );
        t.WriteLine( "Connection: close" );
        // t.WriteLine( "Content-Length: 131" ); 
        t.WriteLine( "" );
        t.Flush();

     Console.WriteLine( "begin send response-body" );
     var b = new BinaryWriter(stream);
        // write data!!!
     b.Write( data );
     b.Flush();

     Console.WriteLine( "end send response" );
}

private void sendResponse( NetworkStream stream )
{
  Console.WriteLine( "begin send response" );
  StreamWriter writer = new StreamWriter( stream );
  
  writer.WriteLine( "HTTP/1.0 200 OK" );
  writer.WriteLine( "Content-Type: text/html; charset=UTF-8" );
  writer.WriteLine( "Server: proxy-win/0.1" );
  writer.WriteLine( "Connection: close" );
  // writer.WriteLine( "Content-Length: 131" ); 
  writer.WriteLine( "" );
  writer.WriteLine( "<html>" );
  writer.WriteLine( "<head>" );
  writer.WriteLine( "<title>An Example Page</title>" );
  writer.WriteLine( "</head>" );
  writer.WriteLine( "<body>" );
  writer.WriteLine( "Hello World, this is a very simple HTML document.");
  writer.WriteLine( "</body>" );
  writer.WriteLine( "</html>" );
  writer.Flush();

  Console.WriteLine( "end send response" );
}


static void Main(string[] args) 
{
  string winProxyAddress = Environment.GetEnvironmentVariable( "WIN_PROXY" );
  if( winProxyAddress == null )
  {
    Console.WriteLine( "*** error - WIN_PROXY env variable missing, please set e.g.:");
    Console.WriteLine( "  $ set WIN_PROXY=http://proxy.bigcorp:57416");
    Environment.Exit( 1 );  // NOTE: 0 is OK; 1..N is ERROR
  }

  int port = 3333;
  Console.WriteLine( "start web proxy server on port " + port );
  Console.WriteLine( "  using WIN_PROXY >>" + winProxyAddress + "<<");

  Server srv = new Server( port );
  srv.ListenAndServe();

  Console.WriteLine( "bye" );
}

} // class Server
