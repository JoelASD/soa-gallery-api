# SOA Gallery API

Image Gallery API for the Service Oriented Applications Course in JAMK

# API Endpoint Docs

## Images

### List images with pagination

**Method:** POST

**Route:** /image

**Parameters:** PageNumber, PageSize

**Headers:** Content-Type: JSON

**Example**

http://localhost:5000/image?PageNumber=1&PageSize=2

**Response**
```json
{
  "pageNumber": 1,
  "pageSize": 2,
  "firstPage": "http://api.imager.local:8765/Image?pageNumber=1&pageSize=2",
  "lastPage": "http://api.imager.local:8765/Image?pageNumber=10&pageSize=2",
  "totalPages": 10,
  "totalRecords": 20,
  "nextPage": "http://api.imager.local:8765/Image?pageNumber=2&pageSize=2",
  "previousPage": null,
  "data": [
    {
      "imageId": "ca482ebc-eeb7-4890-ae59-e5689f902e69",
      "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
      "user": null,
      "imageFile": "10f82e64-95c7-453a-b64e-5075c8ab58d3.png",
      "imageTitle": "jokukuva",
      "isPublic": false,
      "voteSum": 0,
      "comments": null
    },
    {
      "imageId": "f6a3455c-f5ec-4540-beaf-ab3c6349bb05",
      "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
      "user": null,
      "imageFile": "a0bbd69f-ea5b-4b09-bf1a-1ce70db2dc6d.png",
      "imageTitle": "Rave",
      "isPublic": false,
      "voteSum": 1,
      "comments": null
    }
  ],
  "succeeded": true,
  "errors": null,
  "message": null
}
```

<br/>
<br/>
<br/>
<br/>

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

* GET /me/favorites *
* GET /me/favorites/export *
* GET /me/comments *

* POST /auth/register *
* POST /auth/login *
* GET /auth/logout *

* Logging to external server

* Tests
* Favorites as ZIP *
* Google / Facebook login *
* Admin / normal user / anonymous
* Public / Private images *
* Report this image feature

* CI/CD *
* docker-compose.yml with env variables *

* Documentation

* GET /ping *

## Database Model

![Database](images/LogicalDbModel.png)

## Dotnet commands

* Installing Entity Framework: ```dotnet tool install --global dotnet-ef```
* Creating Database Migrations: ```dotnet-ef migrations add InitialCreate```
* Updating Database: ```dotnet-ef database update```
* Starting the server: ```dotnet run``` runs on http://localhost:5001
* Watching the server: ```dotnet run watch``` runs on http://localhost:5001

## Google Auth
* Client ID: 845287079380-rarn3p0kk316olvrq966ca2dkcs1ran1.apps.googleusercontent.com
* Client Secret: 4scH6tm6pwtMgFYOLrQpECf6