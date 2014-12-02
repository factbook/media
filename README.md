# web-proxy-win

Simple personal (authenticaton) web proxy for Windows (in C#/.NET) -
supports Integrated Windows Authentication (IWA)
 a.k.a. Kerberos, HTTP Negotiate, SPNEGO, GSSAPI 'n' friends.

Note: For now supports only HTTP (not HTTPS) and only supports GET requests.


## Usage

If you're behind a corporate Windows proxy that only allows (supports) Kerberos authentication you can use
the `proxy.exe` as a kind of "front-end authentication web proxy".

### Step 1: Set the `WIN_PROXY` environment variable

Set the `WIN_PROXY` environment variable to your corporate proxy e.g.:

    SET WIN_PROXY=http://proxy.bigcorp:57416

Note: Do NOT include your Windows account credentials (login/password). The C#/.NET code uses the defaults e.g.:

    WebProxy proxy = new WebProxy( "http://proxy.bigcorp:57416" );
    proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

### Step 2: Set the `HTTP_PROXY` environment variable

Set the `HTTP_PROXY` environment variable to point to your new simple personal (authentication) web proxy
running on your machine on loopback (`127.0.0.1`) e.g.:

    SET HTTP_PROXY=http://127.0.0.1:3333

###  Step 3: Startup the web proxy

    $ proxy.exe

That's it. Now you can use your command line tools of choice (e.g. `gem`, `npm`, `bower`, `curl`, etc.)
again to fetch documents, archives and more from the internet.


## References

### Wikipedia
- [Integrated Windows Authentication](http://en.wikipedia.org/wiki/Integrated_Windows_Authentication)
- [Kerberos](http://en.wikipedia.org/wiki/Kerberos_(protocol))
- [Security Support Provider Interface (SSPI)](http://en.wikipedia.org/wiki/Security_Support_Provider_Interface) 
- [Simple and Protected GSSAPI Negotiation Mechanism (SPNEGO)](http://en.wikipedia.org/wiki/SPNEGO)
    - [Generic Security Services Application Program Interface (GSSAPI)](http://en.wikipedia.org/wiki/Generic_Security_Services_Application_Program_Interface)


## License

The `web-proxy-win` scripts are dedicated to the public domain.
Use it as you please with no restrictions whatsoever.

