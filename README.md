---
services: active-directory
platforms: dotnet
author: dstrockis
---

# Demo - To-do list reimagined iOS app
This code sample is one of three referenced in the Azure AD sessions of the [Microsoft Cloud Roadshow](https://www.microsoftcloudroadshow.com/).  Recordings of these sessions will be available shortly [here]().  We recommned you watch one of these recordings to understand the purpose and goals of this code sample.

To-Do List Reimagined (tdlr;) is a new cloud service that allows users to store and manage a list of tasks.  It integrates with Azure AD in order to provide enterprise features to its customers that have an existing Azure AD tenant.  These features include:

- Discovery of accounts with an existing Azure AD tenant
- Signing up for the app with a work account
- One-click user authorization using consent & OAuth 2.0
- Signing into the app with a work account
- Sharing tasks between users in the same company using a "people picker".
- Outsourced user management to company admins

The full service consists of three different sample projects:

- [The tdlr; web application](https://github.com/azureadsamples/azureroadshow-web), written as .NET 4.5 MVC app.
- [This tdlr; iOS application](https://github.com/azureadsamples/azureroadshow-xamarin), written as a cross-platform Xamarin app.
- [The tdlr; admin web portal](https://github.com/azureadsamples/azureroadshow-web-admin), written as a .NET 4.5 MVC app.

## Running the tdlr; iOS app

### Get Xamarin

The tdlr; app will run on iOS or Android. If you want to run on iOS you will need a mac on which to build the code.  In addition, you will need to [download & install Xamarin](http://xamarin.com/download) on your machine.  This code sample was built using Xamarin Studio, which is installed by default when your install Xamarin on your machine.  You are free to use Visual Studio on a Windows machine and remote connect to the Xamarin.iOS build host via wifi - but setting up your development environment will be a bit more involved.

### Running the tdlr; web service

This sample Xamarin application is written to be used with the tdlr; web app as its backend data storage service.  Before proceeding, you will need to create your own instance of the service by following the instructions for the [tdlr web app](https://github.com/azureadsamples/azureroadshow-web) and host it somewhere where your device app can communicate with it - we recommend deploying it as an Azure web app.

If you don't want to spin up your own tdlr; service, you can instead use our instance as a workaround.  To sign up for our tdlr; service, open [this link](http://todolistreimagined.azurewebsites.net/account/signup/aad?sign_up_hint=) in a new tab.  Sign in with a user in your development tenant.  This process will add our tdlr; service to your tenant so you can configure your app to access it.  Note that **you will need to repeat this process for each tenant** that you use to sign into device app (we told you it was a workaround).

### Register an app with Azure AD

You'll now need to register your Xamarin app in the Azure Portal so that your version of tdlr; can sign users in and get information from Azure AD. You'll need an Azure Activce Directory tenant in which to register your application. For more information on how to get an Azure AD tenant, please see [How to get an Azure AD tenant](https://azure.microsoft.com/en-us/documentation/articles/active-directory-howto-tenant/). You may also wish to create an additional tenant, since the tdlr; app is 'multi-tenant' - it allows users from any organization to sign up & sign in.  You'll want to create a few users in your tenant(s) for testing purposes - a guest user with a personal MSA account will not work for this sample.

1. Sign in to the [Azure portal](https://portal.azure.com).
2. On the top bar, click on your account and under the **Directory** list, choose the Active Directory tenant where you wish to register your application.
3. Click on **More Services** in the left hand nav, and choose **Azure Active Directory**.
4. Click on **App registrations** and choose **Add**.
5. Enter a friendly name for the application, for example 'TDLR;' and select 'Native' as the Application Type. For the redirect URI, enter `http://tdlr`. Click on **Create** to create the application.
6. While still in the Azure portal, choose your application, click on **Settings** and choose **Properties**.
7. Find the Application ID value and copy it to the clipboard.
8. Configure Permissions for your application - in the Settings menu, choose the 'Required permissions' section, click on **Add**, then **Select an API**, and select 'Microsoft Graph' (this is the Graph API). Then, click on  **Select Permissions** and select 'Read Directory Data'. Also in the same section, locate and click on the tdlr; web app (you can type it in the search box to locate), and click the bottom check mark to add the application.  Select "Access TDLR;" from the "Delegated Permissions" dropdown, and save the configuration.

Note: if you can't find the TDLR; web app in this menu, read the above section.  You'll first need to spin up your own instance of the tdlr; web service, or sign up for ours.

### Modify the registration of the tdlr; web app

In order for the Xamarin app to work across many tenants, you will need to explicitly bind the client app registration in Azure AD with the registration for the web API. You can do so by adding the "Application ID" of the Xamarin app, to the manifest of the web app.

Note: if you are using our instance of the tdlr; service, you can skip this section.  But remember you will have to perform the workaround described above for every tenant which you want to use to sign into the app.

1. Retrieve the application manifest file for the tdlr; web app you registered
    2. returning to the tdlr; web app application page in the Azure portal
    3. click the "Manage Manifest" option at the bottom of the page, and select "Download Manifest"
    4. save the manifest and open it for editing

2. In the manifest, locate the `knownClientApplications` array property, and add the Application ID for your Xamarin app as an element. Your code should look like the following after you're done:
    `"knownClientApplications": ["94da0930-763f-45c7-8d26-04d5938baab2"]`
3. Save the tdlr; web app manifest back to your Azure AD tenant by
    4. returning to the tdlr; application page in the Azure portal
    5. click the "Manage Manifest" option at the bottom of the page, and select "Upload Manifest"
    6. browse to the text file you updated in step 2 and upload it

### Download the code

Now you can [download this repo as a zip](https://github.com/AzureADSamples/azureroadshow-xamarin/archive/master.zip) or clone it to your local machine:

`git clone https://github.com/Azure-Samples/active-directory-xamarin-demo-tdlr`

In your local repo, open the `tdlr.sln` file in Xamarin Studio.

### Edit the app's config

To run the app, you'll need to enter the information from your app registration.  In Xamarin Studio, open the `tdlr/tdlr.cs` file in the root of the project and locate the app config properties.  Replace the following values with your own:

```
		// App Config Values
		public static string clientId = "[Enter your client ID as registered in the Azure Management Portal, e.g. 3d8c4803-ffcd-4b2a-baec-05056abdc408]";
		public static string taskApiResourceId = "https://strockisdevtwo.onmicrosoft.com/tdlr";
```

Also open the `tdlr/utils/TaskHelper.cs` file, and edit the value of the `taskApi` property to the base address of your tdlr web app.  

Note: if you are using our instance of the tdlr; service, leave the default value of the `taskApiResourceId` and `taskApi` as they are.

### Run the app!

You can now run the tdlr; app and explore its functionality.  Try signing up and signing in with your Azure AD users, creating tasks, and sharing them with other users.  To understand the code behind the app, we recommend you watch one of the recorded Microsoft Cloud Roadshow sessions which will be available soon [here]().  If you're already familiar with Azure AD, you may find the code comments instructive as well.
