# HornetSecurityWeatherSearchApp
Simple weather app with Auth (2fa and Jwt Token) 


.Net 8 with EF Core with Code First Approach

Auto Mapper for Mapping

Redis

SMTP Email Client

Serilog for logging

SQl Server - My Sql

NTier Architecture - Controller - Service - Domain

SOLID Principles and Clean Code

JWT for Auth stored in the HTTP Only Cookie and 2fa using Email with Security Code

Localhost cert for Angular APP

In the solution Folder - Acceptence Criteria and Sql Selects

Rate Limiter on the API

Global Error Handler both on Front and Backend Side

Project Init
Install Redis ( im using WSL instalation )
https://redis.io/docs/install/install-redis/install-redis-on-windows/
run this in terminal if you decide to install using (WSL - Ubuntu)
wsl
sudo servoce redis-server start to start the redis 
redis-cli FLUSHDB(to drop everything from redis)

Update-Database
start the solution 

open the Angular Client and run ng serve -ssl to start the solution

Register a User and confirm the user account with the link provided in the email
Login and Use the App

PROJECT DESCRIPTION
Every form that is filled out there is a validation with regex on the frontend Angular client and  also have a validation on backend using regex. On the backend side there are couple of techniques to protect the solutioon like  RATE LIMITER, JWT AUTH (IS STORED IN THE HTTP ONLY COOKIE). SMTP EMail Client and Redis are implemented to help out with TWO Factor Auth. For sending the links to the user for registering the account using a technique with hashing the link (the hash is unique later by the has we identify the user). Also there is a implementation of Serilog to log the errors. From audit there is only a  audit of the logged users that we are saving in the database.   
From the frontend side Auth Guard is present and it is guarding for now two routes and because the token is stored in the http only cookie it is making a call to the api to check if the user is authenticated or not. Also there is a logic for preserving the User login so if the cookie is present instead of hitting the login page the app will directly navigate to the home page. There is a interceptor that is adding with credentials : true to every request in order to get the cookie from the HTTP Only Cookie and send it to the API. Also there is a Global Error Handler for catching errors on both sides on Backend and Frontend


