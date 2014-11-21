Auth0 OAuth
================
This demonstrates adding authentication to your site using Auth0 oauth2.0.

  - Create [the Auth0 application](https://app.auth0.com/#/applications).  
    - After creation, navigate to your Auth0 application Settings tab.  Take note of Domain, Client ID and Client Secret.  You will need these info in next step.
    
  - Deploy this repository to Azure by clicking <a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>
    - Choose `Directory`, `Subscription` and `Location` appropriately.
    - Change the `Site Name` to something you can recognize.  
    - Enter Auth0 Client ID as `Auth0OAuthClientId` value.
    - Enter Auth0 Client Secret as `Auth0OAuthClientSecret` value.
    - Enter Auth0 Domain as `Auth0OAuthDomain` value.
    - Click Next and Deploy to Azure WebSites.  
    - The site url will be `http://{sitename}.azurewebsites.net/`.  Note: `{sitename}` is the `Site Name` you pick above.
    
  -  On Api Settings section of [the live application](https://account.live.com/developers/applications).  Add below comma-separated urls as `Allowed Callback URLs`
    - `http://{sitename}.azurewebsites.net/login/callback,http://{sitename}.azurewebsites.net/logout/complete`

Try browsing to http://{sitename}.azurewebsites.net/.
