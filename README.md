# SOA Gallery API

Image Gallery API for the Service Oriented Applications Course in JAMK

## API Endpoints

* POST /image *
* GET /image?limit=10&page=1 *
* GET /image/trending *
* DELETE /image/:image-id *
* PUT /image/:image-id/vote *
* PUT /image/:image-id/favorite*
* POST /image/:image-id/comment *

* PUT /comment/:comment-id *
* DELETE /comment/:comment-id *

* GET /user/:user-id/image *

* GET /me/favorites
* GET /me/favorites/export
* GET /me/comments

* POST /auth/register *
* POST /auth/login *
* GET /auth/logout

* Logging to external server

* Tests
* Favorites as ZIP
* Google / Facebook login
* Admin / normal user / anonymous
* Public / Private images
* Report this image feature

* CI/CD
* docker-compose.yml with env variables

* Documentation

* GET /ping *

## Database Model

![Database](images/LogicalDbModel.png)

## Dotnet commands

* Installing Entity Framework: ```dotnet tool install --global dotnet-ef```
* Creating Database Migrations: ```dotnet-ef migrations add InitialCreate```
* Updating Database: ```dotnet-ef database update```
* Starting the server: ```dotnet run``` runs on http://localhost:5001