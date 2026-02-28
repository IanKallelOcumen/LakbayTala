FIGMA AUTH – 403 FORBIDDEN FIX
=============================

If you get "Can't auth. Status: 403 Message: Error: Forbidden" with either
token sign-in OR "Sign in with Web Browser":

1. USE A FRESH PERSONAL ACCESS TOKEN (most reliable)
   - In Figma: click your profile (top-left) → Settings → Security.
   - Under "Personal access tokens" click "Generate new token".
   - Give it a name, set expiry, and enable scope "File content" (read files).
   - Copy the token immediately (you won't see it again).
   - In Unity: FCU Inspector → Open settings → FIGMA AUTH tab → paste token
     in the big text field → "Sign In With Access Token".
   - Or put it in figma_local.json as "accessToken" and run
     LakbayTala → Figma → Sign in and set project from figma_local.json.
   - The converter will try both X-Figma-Token and Bearer automatically if
     the first attempt returns 403.

2. IF BROWSER SIGN-IN ALSO RETURNS 403
   - The Figma Converter's built-in OAuth app may be limited by Figma
     (e.g. not re-published for new policies). Use a personal access token
     instead (step 1).

3. CHECK YOUR FIGMA ACCOUNT
   - Make sure you're logged into the correct Figma account that can access
     the file you want to import.
   - The file must be in your account or in a team you belong to.

4. HTTPS
   - In FCU Main Settings, ensure HTTPS is enabled (required by Figma).
