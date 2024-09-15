# Courier notifications

This is a very simple, open source, real time messaging system. It does not save the messages anywhere, being designed to be used on churches or venues, by volunteers (with access control), to send alerts or messages to a big screen. I designed it to be used specifically on my local church. Feel free to use it wherever you want.

## Disclaimers

### Localization

- For now this app is only available in Portuguese (BR) due to time restrictions
- English will be available some time in the future, maybe.

### Security 🚨

Even though I did my due diligence to secure the connections, communication and access control, this app security has **NOT** been properly tested, please do **NOT** use it to send sensitive data anywhere. **You've been warned.**

## How it works

This app has 3 parts:
### The Windows app
Also known as the display client. This is where the management happens, this app controls what messages can be sent (channels). Defines when the message content is valid, define dynamic fields and control user access. It is also responsible to enqueue and display the received messages (which are on memory only and not stored anywhere). Open the configuration panel to define properties such as the server API and Web App URL

### The backend API
It's a hidden layer that houses the simple, file-based database and API endpoints. It also manages the WebSocket connections to the display client (the windows app). Forwarding any new messages to the appropriate clients

### The web app front-end
It's a Blazor WASM PWA web page, it first must get access to a channel by reading the "user" QR Code on the windows display client, then the app connects to the API to get the channel's field data and then it is able to send messages when the windows display client is also running.

## Install / How to

The Windows client app can be downloaded on the Releases page. Just unzip the client and you're ready to go.
Open the client and set the API and App URLs on the configuration panel, then create a new channel, set the fields then invite the users to the channel.

The backend API and Web App (front-end) must be deployed on a server, with a reverse proxy configured with SSL. see the Deployment section below for details

## Deployment setup

### Docker & Your reverse proxy
Courier notifications need two containers running, one serves the front-end web assembly app, the other is the backend API.
I highly recommend using a subdomain for this app. in this readme, I'll use `courier`
When deploying, use nginx or any other reverse proxy to redirect the services like this:
- `https://courier.your-domain.com/` -> to the frontend service (http port 5091)
- `https://courier.your-domain.com/api` -> to the backend service (http port 5090)

**Don't forget to enable WebSocket support on your proxy.**
#### A note on SSL
The front and back-end reverse proxy **MUST** have SSL configured to enable saving the web app as a PWA on the user's phone.
You can do this easily on your reverse proxy if you have a domain. Search for `let's encrypt` for more info
#### Docker images deploy
Use the following docker compose file to deploy the backend and frontend images. Modify the environment variables as needed

``` yaml
services:
  courier-backend:
    image: leocb/cn-server:latest
    container_name: courier-backend
    environment:
      - CORS_HOSTNAME=https://courier.your-domain.com #Enable CORS protection on the API. set this to your front-end URI
      - ENABLE_SWAGGER=false #Set true if you want to explore the endpoints, access via https://courier.your-domain.com/swagger
    volumes:
      - /path/to/database:/database
    ports:
      - 5090:8080 # HTTP port host:container
    restart: unless-stopped


# This container is not compatible with cloudflare tunnels (don't know why)
# The most easy alternative is to use Cloudflare pages (instructions down below).
# If you'll use cloudflare pages to publish the front-end, remove the following service from this compose file:
  courier-frontend:
    image: leocb/cn-web-app:latest
    container_name: courier-frontend
    volumes:
      - /path/to/appsettings.json:/usr/share/nginx/html/appsettings.json # the contents of this file are listed below
    ports:
      - 5091:80 # HTTP port host:container
    restart: unless-stopped


## Front-end config file content (remove the #'s):
#{
#  "ApiHostname": "https://courier.your-domain.com/"
#}

```

#### Front-end web app additional setup
Create a file named `appsettings.json` and save it somewhere on your local machine, then in the docker compose file above, specify where this file is. here's the content for the file, modify where needed:
```json
{
  "ApiHostname": "https://courier.your-domain.com/"
}
```

#### Reverse proxy
Now configure your reverse proxy like this
- `https://courier.your-domain.com/` -> to the frontend service (http port 5091)
- `https://courier.your-domain.com/api` -> to the backend service (http port 5090). 

**Don't forget to enable WebSocket support for the backend proxy!**

Open the display client on windows, configure the backend URI

Finally, go to _courier.your-domain.com_ on your phone, follow the instructions and enjoy!

### Deploy front-end on Cloudflare pages

This is an excerpt from [This tutorial](https://developers.cloudflare.com/pages/framework-guides/deploy-a-blazor-site/), always check it for the most up-to-date info

1. Log in to the [Cloudflare dashboard ↗](https://dash.cloudflare.com/) and select your account.
2. In Account Home, select **Workers & Pages**.
3. Select **Create** > **Pages** > **Connect to Git**.

Select the new GitHub repository that you created and, in the **Set up builds and deployments** section, provide the following information:

> ⚠️ Warning: Notice that the build command below includes the **API URI** and it's different from the front-end sub domain (namely it's `courier-api`), this is because when using the Cloudflare pages, it's not possible to use the same sub-domain for both the back and front-end, since you can't customize the reverse proxy. Use the API on another sub domain instead and you should be fine! 

| Configuration option | Value                                                                               |
| -------------------- | ----------------------------------------------------------------------------------- |
| Production branch    | `latest`                                                                            |
| Build command        | `chmod +x CN.Web.App/build.sh && ./CN.Web.App/build.sh courier-api.your-domain.com` |
| Build directory      | `CN.Web.App/publish/wwwroot`                                                        |

After configuring your site, you can begin your first deploy. You should see Cloudflare Pages installing `dotnet`, your project dependencies, and building your site, before deploying it.

After deploying your site, you will receive a unique subdomain for your project on `*.pages.dev`. Every time you commit new code to your Blazor site, Cloudflare Pages will automatically rebuild your project and deploy it.

---

## Development

The following section has some details needed to properly develop and Build/Publish the project.
To develop the solution, simply use visual studio (2022 or later) and open the `.sln` file
## Publishing

The backend is a ASP.NET project and runs with the kestrel web server, meaning a simple publish action on VS is enough.

The frontend is just a bunch of static files that need a static file server to work. We use nginx and build it using docker. See [here](https://codepruner.com/how-to-run-blazor-wasm-app-in-container/). The front-end can also be deployed using Cloudflare pages.

The Desktop app is a classic C# .NET WPF app. build the release and zip the results to redistribute. 

### Building and publishing the frontend image

#### Docker

> Note: the front-end docker image does not work when using Cloudflare tunnels (don't ask me why), you can use Cloudflare pages instead.

In a command line, Open the `CN.Web.App` folder
Run `dotnet publish -c Release -o ./publish`
Run `docker build -t [username]/cn-web-app .`
To test locally, Run `docker run -it --rm -d -p 8080:80 --name courier-frontend [username]cn-web-app`
Push to docker: `docker image push [username]/cn-web-app`

### Building and publishing the backend image

When publishing the docker images, pay special attention to a bug on VS tooling - Relevant bug reports: [here](https://developercommunity.visualstudio.com/t/Docker-publish-says-successful-when-in-f/10619667) and [here](https://developercommunity.visualstudio.com/t/Cannot-Publish-AspNet-Core-API-to-Docke/10428640)

1. Right click the `CN.Server` project and select `Publish`
2. Select or Create a new profile
	1. If Creating: Select `Docker Container Registry` as the target
	2. Select `Docker Hub` on the specific target
	3. Fill in your username and password
	4. Select `.NET SDK` on the container build
	5. Finish
	6. Edit the generated "publish profile file" and remove your username from the url
3. With the profile selected, Click on the `Publish` button on the top right
4. The output will say "Publish Completed", but in fact **it was not!**
5. Copy the last command before the success messages
6. Open the windows power shell terminal and paste the command
7. Modify the `--repository` argument and add your docker hub `username` and a `/` and execute the command. The image should be uploaded properly now.
8. The command should look something like this

``` powershell
dotnet "C:\Program Files\dotnet\sdk\8.0.200\Containers\build\..\containerize\containerize.dll" D:\Code\Courier-Notifications\CN.Server\obj\Release\net8.0\linux-x64\PubTmp\Out --baseregistry mcr.microsoft.com --baseimagename dotnet/aspnet --repository leocb/cn-server --workingdirectory /app --baseimagetag 8.0 --outputregistry registry.hub.docker.com --appcommand dotnet CN.Server.dll --labels org.opencontainers.image.created=2024-07-03T03:55:20.7131052Z org.opencontainers.artifact.created=2024-07-03T03:55:20.7131052Z org.opencontainers.image.authors=leocb org.opencontainers.image.version=1.0.0 "org.opencontainers.image.title=Courier Notifications Server" org.opencontainers.image.base.name=mcr.microsoft.com/dotnet/aspnet:8.0 --imagetags latest --rid linux-x64 --ridgraphpath "C:\Program Files\dotnet\sdk\8.0.200/PortableRuntimeIdentifierGraph.json" 
```
