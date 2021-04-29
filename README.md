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
    - [Get top 5 upvoted images from last 24h (trending)](#get-top-5-upvoted-images-from-last-24h-trending)
    - [List all images of the specific user](#list-all-images-of-the-specific-user)
    - [Add image to favorites or remove image from favorites](#add-image-to-favorites-or-remove-image-from-favorites)
  - [Authentication](#authentication)
    - [Authenticate using google account](#authenticate-using-google-account)
    - [Register user](#register-user)
    - [Login](#login)
    - [Logout](#logout)
  - [Comments](#comments)
    - [Comment an image](#comment-an-image)
    - [Edit comment](#edit-comment)
    - [Delete comment](#delete-comment)
  - [Vote](#vote)
    - [Vote image or remove already given vote](#vote-image-or-remove-already-given-vote)
  - [Me](#me)
    - [List all comments user has made](#list-all-comments-user-has-made)
    - [List all images user has favorited](#list-all-images-user-has-favorited)
    - [Export images user has favorited](#export-images-user-has-favorited)
  - [Database Model](#database-model)
    - [Version 1](#version-1)
    - [Version 2](#version-2)
  - [Dotnet commands](#dotnet-commands)
- [Features not implemented](#features-not-implemented)
- [Summary](#summary)
  - [Raw time estimation](#raw-time-estimation)
  - [Difficulty assessment](#difficulty-assessment)
  - [Best parts](#best-parts)

# API Endpoint Docs

## Images

### List images with pagination ordered by descending datetime

**Method:** GET

**Route:** /image

**Response Code:** 200

**Parameters:** PageNumber, PageSize

**Example**

http://galleryapi.codesamson.com/image?PageNumber=1&PageSize=2

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
      "isPublic": true,
      "created": "2021-04-24T19:11:33.691868",
      "voteSum": 0,
      "comments": null
    },
    {
      "imageId": "f6a3455c-f5ec-4540-beaf-ab3c6349bb05",
      "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
      "user": null,
      "imageFile": "a0bbd69f-ea5b-4b09-bf1a-1ce70db2dc6d.png",
      "imageTitle": "Rave",
      "isPublic": true,
      "created": "2021-04-24T19:11:33.691868",
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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/image/all

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

**Response Code:** 201

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

http://galleryapi.codesamson.com/image

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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Body:**
```json
{
  "imageId": "string",
  "imageTitle": "string"
}
```

**Example**

http://galleryapi.codesamson.com/image

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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/image/{id}/

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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/image/{image_id}/

**Response**
```json
{
    "data": null,
    "succeeded": true,
    "errors": null,
    "message": "Image e4512723-06b2-4229-a223-aac2ed909f96.png was deleted with its votes and comments"
}
```


### Get top 5 upvoted images from last 24h (trending)

**Method:** GET

**Route:** /image/trending

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/image/trending/

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
            "isPublic": true,
            "created": "2021-04-24T19:11:33.691868",
            "voteSum": 1,
            "comments": null
        },
        {
            "imageId": "ca482ebc-eeb7-4890-ae59-e5689f902e69",
            "userId": null,
            "user": null,
            "imageFile": "10f82e64-95c7-453a-b64e-5075c8ab58d3.png",
            "imageTitle": "jokukuva",
            "isPublic": true,
            "created": "2021-04-24T19:11:33.691868",
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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/user/{user_id}/images/

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
            "isPublic": true,
            "created": "2021-04-24T19:11:33.691868",
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "b4794e09-0719-443a-b509-9eaecfe10eb3",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "9895891a-2b36-49d9-91c3-646009c42928.jpg",
            "imageTitle": "Iron Man",
            "isPublic": true,
            "created": "2021-04-24T19:11:33.691868",
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "20dc6eaf-03e4-4bed-b1fe-d9636a7fd1e7",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "0a12aeb1-6e5f-4670-8ac0-8fdf7c24c143.jpg",
            "imageTitle": "Iron Man",
            "isPublic": true,
            "created": "2021-04-24T19:11:33.691868",
            "voteSum": 0,
            "comments": null
        },
        {
            "imageId": "3068a915-cc9f-48c5-a3fb-584953420116",
            "userId": "548406ef-48c8-4c76-8fee-55b52d7b1e1a",
            "user": null,
            "imageFile": "5050f037-e35a-4e18-afcb-3aa12b6050f7.jpg",
            "imageTitle": "Iron Man",
            "isPublic": true,
            "created": "2021-04-24T19:11:33.691868",
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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/image/{image_id}/favorite/

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

**Response Code:** 200

**Callback route:** /signin-callback

**Example**

http://galleryapi.codesamson.com/google/

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

**Response Code:** 200

**Body:**

```json
{
  "email": "user@example.com",
  "userName": "string",
  "password": "string length min: 8, 1 special char, one capital and 1 number"
}
```

**Example**

http://galleryapi.codesamson.com/register/

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

**Response Code:** 200

**Body:**

```json
{
  "email": "user@example.com",
  "password": "string"
}
```

**Example**

http://galleryapi.codesamson.com/login/

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

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/logout/

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

## Comments

### Comment an image

**Method:** POST

**Route:** /image/{image-id}/comment

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Body:**

```json
{
    "commentText": "string"
}
```

**Example**

http://galleryapi.codesamson.com/image/{image-id}/comment

**Response**

```json
{
    "data": {
        "commentId": "{new comment id}",
        "user": {
            "userId": "{poster id}",
            "userName": null
        },
        "commentText": "string",
        "imageId": "{image id}"
    },
    "succeeded": true,
    "errors": null,
    "message": "Comment added"
}
```

### Edit comment

**Method:** PUT

**Route:** /comment/{comment-id}

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Body:**

```json
{
    "commentText": "string"
}
```

**Example**

http://galleryapi.codesamson.com/comment/{comment-id}

**Response**

```json
{
    "data": {
        "commentId": "{new comment id}",
        "user": {
            "userId": "{poster id}",
            "userName": null
        },
        "commentText": "string",
        "imageId": "{image id}"
    },
    "succeeded": true,
    "errors": null,
    "message": "Comment edited!"
}
```

### Delete comment

**Method:** Delete

**Route:** /comment/{comment-id}

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/comment/{comment-id}

**Response**

```json
{
    "data": null,
    "succeeded": true,
    "errors": null,
    "message": "Comment deleted!"
}
```

## Vote

### Vote image or remove already given vote

**Method:** Put

**Route:** /image/{image-id}/vote

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Body:**
Voted only accepts -1 and 1

```json
{
    "Voted": 1
}
```

**Example**

http://galleryapi.codesamson.com/image/{image-id}/vote

**Response**

```json
{
    "data": {
        "voteId": "{id}",
        "voted": -1,
        "userId": "{id}",
        "imageId": "{id}"
    },
    "succeeded": true,
    "errors": null,
    "message": "Vote added!"
}
```
```json
{
    "data": {
        "voteId": "{id}",
        "voted": 1,
        "userId": "{id}",
        "imageId": "{id}"
    },
    "succeeded": true,
    "errors": null,
    "message": "Vote changed"
}
```
```json
{
    "data": null,
    "succeeded": true,
    "errors": null,
    "message": "Vote deleted"
}
```

## Me

### List all comments user has made

**Method:** GET

**Route:** /me/comments

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/me/comments

**Response**

```json
{
  "data": [
    {
      "commentId": "271ff2e7-945b-44c5-810c-b12c35797e3d",
      "user": {
        "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
        "userName": "joel"
      },
      "commentText": "siisti kuva",
      "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb"
    },
    {
      "commentId": "a5a2bdf4-ead4-4129-be45-64e8f8ef23a9",
      "user": {
        "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
        "userName": "joel"
      },
      "commentText": "siisti!",
      "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb"
    },
    {
      "commentId": "5748d6e7-7b6b-41da-a850-b5913d48c42d",
      "user": {
        "userId": "e9b27b1c-1f8d-4da9-94cf-7e6f59c71ab6",
        "userName": "joel"
      },
      "commentText": "first!",
      "imageId": "c6fd4feb-ef86-4445-8d24-c034e4f08aeb"
    }
  ],
  "succeeded": true,
  "errors": null,
  "message": null
}
```

### List all images user has favorited

**Method:** GET

**Route:** /me/favorites

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/me/favorites

**Response**

```json
{
  "data": [
    {
      "imageId": "d16253d3-c000-457b-815b-91b960287763",
      "userId": null,
      "user": {
          "userId": "0983e612-a6e5-45bb-ae76-1e7c9d87dcc3",
          "userName": "jouni"
      },
      "imageFile": "3a234904-499d-4777-9c88-288856fadabf.png",
      "imageTitle": "testminio",
      "isPublic": true,
      "created": "2021-04-24T19:11:33.691868",
      "voteSum": 0,
      "comments": null
    }
  ],
  "succeeded": true,
  "errors": null,
  "message": null
}
```

### Export images user has favorited

**Method:** GET

**Route:** /me/favorites/export

**Response Code:** 200

**Headers:** Authorization: "Bearer {Token}"

**Example**

http://galleryapi.codesamson.com/me/favorites/export

**Response**

Downloads favorites.zip automatically


<br/>
<br/>
<br/>
<br/>


## Database Model

### Version 1
![Database](images/LogicalDbModel.png)

### Version 2
![Database](images/logical-model-v2.JPG)

## Dotnet commands

* Installing Entity Framework: ```dotnet tool install --global dotnet-ef```
* Creating Database Migrations: ```dotnet-ef migrations add InitialCreate```
* Updating Database: ```dotnet-ef database update```
* Starting the server: ```dotnet run``` runs on http://localhost:5001
* Watching the server: ```dotnet run watch``` runs on http://localhost:5001

# Features not implemented

* Logging to external server
* Tests
* Admin / normal user / anonymous
* Report this image feature


# Summary

Here is the summary or small report for the project.

## Raw time estimation

| Feature                                                                                          | Time (h)  |
| ------------------------------------------------------------------------------------------------ | :-------: |
| Ability to upload a new image                                                                    |     4     |
| Ability to list uploaded images paginated                                                        |    3.5    |
| List uploads per user                                                                            |     2     |
| Ability to remove an uploaded image                                                              |    2.5    |
| Ability to vote (upvote, downvote) an image                                                      |     3     |
| Ability to mark image as a favorite or remove it                                                 |     3     |
| Ability to comment on a image                                                                    |    3.5    |
| Ability to edit or remove comments                                                               |     3     |
| Ability to create a user / register                                                              |     5     |
| Ability to login and logout                                                                      |     4     |
| Able to list most "trending" images                                                              |     4     |
| Ability to download favorites as a zip / export                                                  |     4     |
| Login using a Google / Facebook / etc. account                                                   |     6     |
| Public / private images                                                                          |     3     |
| Usage of Gitlab CI to build a docker image of the API and upload it to Gitlab container registry |     3     |
| Creating a docker-compose.yaml file with the appropriate environment variables defined           |     5     |
| Testing                                                                                          |     8     |
| Bug fixes                                                                                        |    20     |
| Server / CSC configurations                                                                      |    10     |
| Documentation                                                                                    |     5     |
| **Total**                                                                                        | **101.5** |

## Difficulty assessment

We did not find anything very hard, some features were more time consuming to develop that others. The assignment was very enjoyable to do, so nothing seemed too hard.

## Best parts

The greatest learning experience was combined from three parts:
* Learning a new backend development framework: ASP.NET Core
* Learning Docker and Docker Compose
* Learning Gitlab's CD/CI pipelines

Also teamwork deserves an honourable mention.
