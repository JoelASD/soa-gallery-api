# SOA Gallery API

Image Gallery API for the Service Oriented Applications Course in JAMK

* POST /image
* GET /image?limit=10&page=1
* GET /image/trending
* DELETE /image/:image-id
* PUT /image/:image-id/vote
* PUT /image/:image-id/favorite
* POST /image/:image-id/comment

* PUT /comment/:comment-id
* DELETE /comment/:comment-id

* GET /user/:user-id/image

* GET /me/favorites
* GET /me/favorites/export
* GET /me/comments

* POST /auth/register
* POST /auth/login
* GET /auth/logout
