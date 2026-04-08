# Blog API — ASP.NET Core Backend

## Tech Stack
- **Framework**: ASP.NET Core on .NET 10
- **ORM**: Entity Framework Core 9 with SQLite
- **Auth**: JWT Bearer tokens + BCrypt password hashing
- **Docs**: Swagger UI at `http://localhost:5000/swagger`

---

## Folder Structure

```
legacy_backend/
├── Controllers/
│   ├── AuthController.cs       # Register and login endpoints
│   ├── PostsController.cs      # CRUD for blog posts
│   └── CommentsController.cs   # CRUD for comments on a post
│
├── Models/
│   ├── User.cs                 # User entity (id, username, passwordHash)
│   ├── Post.cs                 # Post entity (id, title, content, author, timestamps)
│   └── Comment.cs              # Comment entity (id, content, author, postId, timestamp)
│
├── Data/
│   └── BlogContext.cs          # EF Core DbContext — registers all tables
│
├── Program.cs                  # App entry point — middleware, DI, startup config
├── appsettings.json            # Config — DB connection, JWT settings, CORS origins
├── appsettings.Development.json# Dev-only config overrides (verbose logging)
├── BlogApi.csproj              # Project file — NuGet package dependencies
│
├── blogapi.service             # Systemd service file for Ubuntu EC2
└── deploy.sh                   # Deployment script for Ubuntu EC2
```

---

## Running Locally

```bash
dotnet run
# API available at http://localhost:5000
# Swagger UI at  http://localhost:5000/swagger
```

---

## API Endpoints

| Method | Endpoint                              | Auth Required | Description          |
|--------|---------------------------------------|---------------|----------------------|
| POST   | /api/auth/register                    | No            | Create a new account |
| POST   | /api/auth/login                       | No            | Login, returns JWT   |
| GET    | /api/posts                            | No            | List all posts       |
| GET    | /api/posts/{id}                       | No            | Get a single post    |
| POST   | /api/posts                            | Yes           | Create a post        |
| PUT    | /api/posts/{id}                       | Yes           | Update a post        |
| DELETE | /api/posts/{id}                       | Yes           | Delete a post        |
| GET    | /api/posts/{postId}/comments          | No            | List comments        |
| POST   | /api/posts/{postId}/comments          | Yes           | Add a comment        |
| DELETE | /api/posts/{postId}/comments/{id}     | Yes           | Delete a comment     |

---

## Where to Make Changes

### Change the database
`appsettings.json` → `ConnectionStrings.DefaultConnection`
```json
"DefaultConnection": "Data Source=blog.db"
```
Replace with a different SQLite file path or swap to PostgreSQL/MySQL by changing the provider in `Program.cs` and installing the relevant EF Core package.

### Change the JWT secret key
`appsettings.json` → `Jwt.Key`
```json
"Jwt": {
  "Key": "BlogAppSuperSecretKey123456789012"
}
```
Use a long random string in production. Never commit real secrets to source control.

### Change token expiry
`Controllers/AuthController.cs` → `GenerateToken()` method
```csharp
expires: DateTime.UtcNow.AddDays(7)
```

### Allow a different frontend origin (CORS)
`appsettings.json` → `Cors.AllowedOrigins`
```json
"Cors": {
  "AllowedOrigins": [ "http://localhost:4200" ]
}
```
Add your frontend EC2 IP here before deploying, e.g. `"http://1.2.3.4"`.

### Add a new field to a model
1. Add the property to the model in `Models/`
2. `BlogContext.cs` will pick it up automatically
3. Delete `blog.db` and restart — EF will recreate the schema (dev only)

### Add a new API endpoint
1. Add the method to the relevant controller in `Controllers/`
2. Add `[Authorize]` if it requires login
3. The route is derived from the class route + method attribute

### Change the listening port
`appsettings.json` or set the environment variable before running:
```bash
ASPNETCORE_URLS=http://0.0.0.0:8080 dotnet run
```
Also update the systemd service file `blogapi.service` → `Environment=ASPNETCORE_URLS`.

---

## Deploying to Ubuntu EC2

1. Edit `deploy.sh` and set `FRONTEND_EC2_IP` to your frontend instance's public IP
2. Copy the folder to the EC2 instance
3. Run:
```bash
bash deploy.sh
```
The script installs .NET, builds the app, configures CORS, and registers it as a systemd service that starts on boot.
