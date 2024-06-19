# Sincerely from Daniil

Hello, thank you for your time and for this task, it was interesting. 

To start a project you need to use docker, write this command below, and you ready to go, all tables will be created for you
    
    `docker run --name mysql-container -e MYSQL_ROOT_PASSWORD=password -e MYSQL_DATABASE=paylocity -d mysql:latest`

- I've implemented the main part of this code challenge - paycheck calculations and added for this unit testing 
- I started to implement things like creating Employees and Dependents and update them
- I need to add better validation for creation Employee, just like I've added them for `AddDependent` EP method
- I need to debug some integration tests to figure out what happens bad(While testing manually it works :D)
- I used in memory integration testing to make it independent for testing, we don't need to start app to test it
- It is possible to implement more services to make Controllers more clean
- In many Dto or Models I used `init;` to prohibit mutating in middle of nowhere during method execution
- Used classic approach of implementing classes, using interfaces to describe behaviour of class
- Unfortunately almost forgot to use `ShoulExtension.cs`, but must say it is nice
- Of course I should hide DB secrets in case of making this App as a prod. version

________________________________________________________________________________________________________________________

# What is this?

A project seed for a C# dotnet API ("PaylocityBenefitsCalculator").  It is meant to get you started on the Paylocity BackEnd Coding Challenge by taking some initial setup decisions away.

The goal is to respect your time, avoid live coding, and get a sense for how you work.

# Coding Challenge

**Show us how you work.**

Each of our Paylocity product teams operates like a small startup, empowered to deliver business value in
whatever way they see fit. Because our teams are close knit and fast moving it is imperative that you are able
to work collaboratively with your fellow developers. 

This coding challenge is designed to allow you to demonstrate your abilities and discuss your approach to
design and implementation with your potential colleagues. You are free to use whatever technologies you
prefer but please be prepared to discuss the choices you’ve made. We encourage you to focus on creating a
logical and functional solution rather than one that is completely polished and ready for production.

The challenge can be used as a canvas to capture your strengths in addition to reflecting your overall coding
standards and approach. There’s no right or wrong answer.  It’s more about how you think through the
problem. We’re looking to see your skills in all three tiers so the solution can be used as a conversation piece
to show our teams your abilities across the board.

Requirements will be given separately.%
