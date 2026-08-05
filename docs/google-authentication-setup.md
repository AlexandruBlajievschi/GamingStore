# Google Authentication Setup

Gaming Store uses Google's server-side OAuth flow through ASP.NET Core Identity. The Google client secret belongs only in backend secret storage. It must never be added to the frontend, committed to Git, or pasted into an `appsettings*.json` file.

This guide is for contributors running their own local copy and operators deploying their own Gaming Store instance. Store customers do not create Google credentials; they only select **Continue with Google** after the application owner has configured the provider once.

## 1. Create the Google project

1. Open [Google Cloud Console](https://console.cloud.google.com/) and create or select a project.
2. Open **Google Auth Platform**.
3. On **Branding**, set the app name to `Gaming Store`, choose a user-support email, and add your developer contact email.
4. On **Audience**, choose **External** for ordinary Google accounts. Keep the app in **Testing** while developing and add the Google accounts that will test it if the Console asks for test users.
5. On **Data Access**, keep only the basic identity scopes used for sign-in: `openid`, email, and profile. Gaming Store does not request Gmail, Drive, Calendar, or other Google API access.

Google may ask for privacy-policy, terms, homepage, authorized-domain, or brand-verification information before a public production launch. Local development with your own test account does not require adding broader scopes.

## 2. Create the OAuth client

1. Open **Google Auth Platform > Clients**.
2. Select **Create client**.
3. Choose **Web application**. Do not choose Desktop, Android, or iOS.
4. Name it `Gaming Store local development`.
5. Add this authorized JavaScript origin:

   ```text
   http://localhost:3000
   ```

6. Add this exact authorized redirect URI:

   ```text
   http://localhost:3000/api/auth/google/callback
   ```

7. Create the client and retain its **Client ID** and **Client secret**.

The redirect URI is case-sensitive and its scheme, host, port, path, and trailing-slash behavior must match exactly. The Next.js `/api/*` rewrite sends this callback to ASP.NET Core, where the Google authentication middleware consumes it.

## 3. Store the backend credentials

From the repository root, run:

```powershell
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID" --project backend/GamingStore.Api
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET" --project backend/GamingStore.Api
```

ASP.NET Core loads user-secrets only for local development. For staging and production, set the same configuration keys in the host's secret manager. Environment-variable names are:

```text
Authentication__Google__ClientId
Authentication__Google__ClientSecret
```

## 4. Run and test locally

Start the backend on the HTTP profile used by the Next.js rewrite:

```powershell
dotnet run --project backend/GamingStore.Api --launch-profile http
```

In another terminal, start the frontend:

```powershell
npm --prefix frontend run dev
```

Open `http://localhost:3000/login` and select **Continue with Google**. A successful first login creates the local Identity user, stores the Google link, issues the normal Gaming Store HttpOnly session cookie, and returns to the storefront.

For an existing password account with the same email, sign in with the password first and select **Connect Google** from the account menu. Gaming Store deliberately does not merge accounts using email matching alone.

## Production setup

Create a separate production OAuth client and register the public callback, for example:

```text
https://gamingstore.example/api/auth/google/callback
```

Production must use HTTPS. Configure the reverse proxy to forward the original host and protocol, keep credentials in the deployment platform's secret manager, and update Google's Branding, Audience, authorized-domain, and publishing settings before opening sign-in to the public.

Rotate the client secret immediately if it is ever committed, logged, or shared outside trusted secret storage.
