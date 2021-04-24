# SOA Gallery API

**JAMK University of Applied Sciences**

Creators:
* Samson Azizyan
* Joel Aalto


Image Gallery API for the Service Oriented Applications Course in JAMK

Swagger Docs: http://galleryapi.codesamson.com/swagger/index.html

# Table of Contents
- [API Endpoint Docs](#api-endpoint-docs)
  - [Images](#images)
    - [List images with pagination ordered by descending datetime](#list-images-with-pagination-ordered-by-descending-datetime)
    - [List All Images (if admin)](#list-all-images-if-admin)
    - [Add one image](#add-one-image)
    - [Edit image](#edit-image)
    - [Get one image](#get-one-image)
    - [Delete image](#delete-image)
    - [Edit top 5 upvoted images from last 24h (trending)](#edit-top-5-upvoted-images-from-last-24h-trending)
    - [List all images of the specific user](#list-all-images-of-the-specific-user)
    - [Add image to favorites or remove image from favorites](#add-image-to-favorites-or-remove-image-from-favorites)
  - [Authentication](#authentication)
    - [Authenticate using google account](#authenticate-using-google-account)
    - [Register user](#register-user)
    - [Login](#login)
    - [Logout](#logout)
- [Features to do in the future](#features-to-do-in-the-future)
  - [Database Model](#database-model)
    - [Version 1](#version-1)
  - [Dotnet commands](#dotnet-commands)

# API Endpoint Docs

## Images

### List images with pagination ordered by descending datetime

**Method:** GET

**Route:** /image

**Parameters:** PageNumber, PageSize

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

### List All Images (if admin)

**Method:** GET

**Route:** /image/all

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/image/all/

**Response**
```json
{
    "data": [
        {
            "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "1cf1331d-88ce-441b-8dfc-e136b4541834.jpg",
            "imageTitle": "1cf1331d-88ce-441b-8dfc-e136b4541834.jpg",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "b4794e09-0719-443a-b509-9eaecfe10eb3",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "9895891a-2b36-49d9-91c3-646009c42928.jpg",
            "imageTitle": "9895891a-2b36-49d9-91c3-646009c42928.jpg",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "20dc6eaf-03e4-4bed-b1fe-d9636a7fd1e7",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "0a12aeb1-6e5f-4670-8ac0-8fdf7c24c143.jpg",
            "imageTitle": "0a12aeb1-6e5f-4670-8ac0-8fdf7c24c143.jpg",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        }
    ],
    "succeeded": true,
    "errors": null,
    "message": null
}
```

### Add one image

**Method:** POST

**Route:** /image

**Headers:** Authorization: "Bearer {Token}"

**Body:**
```json
{
  "imageFile": "{file_extension},{base64string}",
  "imageTitle": "Iron Man",
  "isPublic": true
}
```

**Example**

http://localhost:5000/image/

**Response**
```json
{
    "data": {
        "id": "d66a6754-ff49-46bc-825e-e8a38627f81a",
        "imageFile": "4b4afa8d-5464-407f-b051-ddf40bc024e0.jpg",
        "imageTitle": "Iron Man",
        "userID": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
        "isPublic": true,
        "user": null,
        "votes": null,
        "favourites": null,
        "comments": null,
        "created": "2021-04-24T20:46:08.3118564+03:00",
        "updated": "2021-04-24T20:46:08.3118623+03:00"
    },
    "succeeded": true,
    "errors": null,
    "message": "Image has been added succesfully"
}
```

### Edit image

**Method:** PUT

**Route:** /image

**Headers:** Authorization: "Bearer {Token}"

**Body:**
```json
{
  "imageId": "string",
  "imageTitle": "string"
}
```

**Example**

http://localhost:5000/image/

**Response**
```json
{
    "data": {
        "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb",
        "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
        "user": null,
        "imageFile": null,
        "imageTitle": "iron man 2",
        "isPublic": false,
        "voteSum": 0,
        "comments": null
    },
    "succeeded": true,
    "errors": null,
    "message": "Image updated successfully"
}
```

### Get one image

**Method:** GET

**Route:** /image/{id}

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/image/{id}/

**Response**
```json
{
    "data": {
        "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb",
        "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
        "user": null,
        "imageFile": "1cf1331d-88ce-441b-8dfc-e136b4541834.jpg",
        "imageTitle": "1cf1331d-88ce-441b-8dfc-e136b4541834.jpg",
        "isPublic": false,
        "voteSum": -2,
        "comments": [
            {
                "commentId": "271ff2e7-945b-44c5-810c-b12c35797e3d",
                "user": {
                    "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
                    "userName": "joel"
                },
                "commentText": "siisti kuva",
                "imageId": null
            },
            {
                "commentId": "a5a2bdf4-ead4-4129-be45-64e8f8ef23a9",
                "user": {
                    "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
                    "userName": "joel"
                },
                "commentText": "siisti!",
                "imageId": null
            },
            {
                "commentId": "5748d6e7-7b6b-41da-a850-b5913d48c42d",
                "user": {
                    "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
                    "userName": "joel"
                },
                "commentText": "first!",
                "imageId": null
            },
            {
                "commentId": "7194b66b-93a6-463c-9100-b7fc4a4e3b8d",
                "user": {
                    "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
                    "userName": "joel"
                },
                "commentText": "lul",
                "imageId": null
            },
            {
                "commentId": "c2e3f927-64eb-47a8-82e1-bbf106c39b88",
                "user": {
                    "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
                    "userName": "joel"
                },
                "commentText": "testicomment",
                "imageId": null
            }
        ]
    },
    "succeeded": true,
    "errors": null,
    "message": null
}
```

### Delete image

**Method:** DELETE

**Route:** /image/{image_id}

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/image/{image_id}/

**Response**
```json
{
    "data": null,
    "succeeded": true,
    "errors": null,
    "message": "Image e4512723-06b2-4229-a223-aac2ed909f96.png was deleted with its votes and comments"
}
```


### Edit top 5 upvoted images from last 24h (trending)

**Method:** GET

**Route:** /image/trending

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/image/trending/

**Response**
```json
{
    "data": [
        {
            "imageId": "83ecff66-59e5-4344-98a5-93ed872fefa1",
            "userId": null,
            "user": null,
            "imageFile": "28b6174f-c91c-4400-8f26-6f0a4a3d64b8.png",
            "imageTitle": "tester image",
            "isPublic": false,
            "voteSum": 1,
            "comments": null
        },
        {
            "imageId": "ca482ebc-eeb7-4890-ae59-e5689f902e69",
            "userId": null,
            "user": null,
            "imageFile": "10f82e64-95c7-453a-b64e-5075c8ab58d3.png",
            "imageTitle": "jokukuva",
            "isPublic": false,
            "voteSum": -1,
            "comments": null
        }
    ],
    "succeeded": true,
    "errors": null,
    "message": null
}
```

### List all images of the specific user

**Method:** GET

**Route:** /user/{user_id}/images

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/user/{user_id}/images/

**Response**
```json
{
    "data": [
        {
            "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "1cf1331d-88ce-441b-8dfc-e136b4541834.jpg",
            "imageTitle": "iron man 2",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "b4794e09-0719-443a-b509-9eaecfe10eb3",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "9895891a-2b36-49d9-91c3-646009c42928.jpg",
            "imageTitle": "Iron Man",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "20dc6eaf-03e4-4bed-b1fe-d9636a7fd1e7",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "0a12aeb1-6e5f-4670-8ac0-8fdf7c24c143.jpg",
            "imageTitle": "Iron Man",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "3068a915-cc9f-48c5-a3fb-584953420116",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "5050f037-e35a-4e18-afcb-3aa12b6050f7.jpg",
            "imageTitle": "Iron Man",
            "isPublic": false,
            "voteSum": 0,
            "comments": null
        }
    ],
    "succeeded": true,
    "errors": null,
    "message": "Success"
}
```

### Add image to favorites or remove image from favorites

**Method:** PUT

**Route:** /image/{image_id}/favorite

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/image/{image_id}/favorite/

**Response**
```json
{
    "data": null,
    "succeeded": true,
    "errors": null,
    "message": "Image has been succesfully removed from favorites"
}
```

## Authentication

### Authenticate using google account

This works locally / development, but on the CSC instance it returns an error. This is probably cause by the lack of the security group rule for the ingress from google server. We could not figure out the ip address that is used by Google for the authentication callbacks

![Google Auth Error](images/google-error.JPG)

**Route:** /google

**Callback route:** /signin-callback

**Example**

http://localhost:5000/google/

**Response**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImYxOGM5YjZhLTc3YjMtNGU5Mi1hYWFlLTc2ZGIyYjc1ZmUxZiIsImVtYWlsIjoiaW90LnR0djE4c0BnbWFpbC5jb20iLCJzdWIiOiJpb3QudHR2MThzQGdtYWlsLmNvbSIsImp0aSI6IjA4MWU1YjI1LTZlMzQtNDQ5Ny04MDNjLWM0YzVhNTUzMmRkYyIsIm5iZiI6MTYxOTI4ODMxOCwiZXhwIjoxNjE5Mzc0NzE4LCJpYXQiOjE2MTkyODgzMTh9.HyYEGyMvATaGY7dhyumuWDapCIeZuM7iXp6PXbNin_0",
  "result": true,
  "errors": null,
  "id": null
}
```

![Google Auth Error](images/google-auth.JPG)

### Register user

**Method:** POST

**Route:** /register

**Body:**

```json
{
  "email": "user@example.com",
  "userName": "string",
  "password": "string length min: 8, 1 special char, one capital and 1 number"
}
```

**Example**

http://localhost:5000/register/

**Response**
```json
{
  "token": "{token}",
  "result": true,
  "errors": null,
  "id": "{userID}"
}
```

### Login

**Method:** POST

**Route:** /login

**Body:**

```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**Example**

http://localhost:5000/login/

**Response**
```json
{
  "token": "{token}",
  "result": true,
  "errors": null,
  "id": null
}
```

### Logout

**Method:** POST

**Route:** /logout

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://localhost:5000/logout/

**Response**

```json
{
    "data": {
        "token": "{token}",
        "created": "2021-04-24T18:31:01.1888423+00:00",
        "expires": "2021-04-25T18:27:56+00:00"
    },
    "succeeded": true,
    "errors": null,
    "message": null
}
```

<br/>
<br/>
<br/>
<br/>

# Features to do in the future

* Logging to external server
* Tests
* Admin / normal user / anonymous
* Report this image feature

## Database Model

### Version 1
![Database](images/LogicalDbModel.png)

## Dotnet commands

* Installing Entity Framework: ```dotnet tool install --global dotnet-ef```
* Creating Database Migrations: ```dotnet-ef migrations add InitialCreate```
* Updating Database: ```dotnet-ef database update```
* Starting the server: ```dotnet run``` runs on http://localhost:5001
* Watching the server: ```dotnet run watch``` runs on http://localhost:5001
