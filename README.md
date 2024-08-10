# Teach Planner - A digital planner for classroom teachers

## Table of Contents

- [Motivation](#motivation)
- [Problem Solved](#problem-solved)
- [Application Structure](#application-structure)
- [Future Goals](#future-goals)
- [Further Information](#further-information)

## Motivation

Classroom teachers spend a significant amount of time doing lesson planning, recording assessment grades and making notes for report writing.

They are also required to either create or source resources to supplement learning.

The purpose of this app is to reduce the amount of time spent on repetitive tasks and to create lesson structures that can easily be modified and reused from year-to-year.

This application is made for my wife who is a primary school teacher. The features and design are based on her needs, but should be applicable to any classroom teacher in South Australia.

## Problem Solved

This project aims to do the following:

<ul>
<li>Enable overviews of the school year, terms and individual weeks at different levels of granularity</li>
<li>Allow daily lesson planning with the ability to easily email that plan and associated resources if the teacher is unwell and needs to provide notes for a reliever</li>
<li>Enable teacher to keep records of grades and other administrative notes about each student</li>
<li>Automatically generate report comments based on grades</li>
</ul>

## Application Structure

The application uses Domain-Driven-Design principles to structure the code. The back end is divided into the following layers:

<ul>
<li>Application - contains the use cases for the application</li>
<li>Domain - contains the entities and value objects for the application</li>
<li>Infrastructure - contains the database and other infrastructure code</li>
</ul>

Use cases are implemented in using the CQRS pattern, and files are organised as vertical slices to keep all related code together.

To get the API to an initially working state, all of the South Australian curriculum documents are parsed into domain models that are stored in the MySQL database. The curriculum is loaded into memory as a singleton by the API and is used as the source of truth for the app.

The front end is build using a standalone Blazor WebAssembly client. Given that the client is written in the same language as the back end, a significant amount of code can be shared when transferring data from the back end to the front end. Styling is done with Tailwind CSS.

## Future Goals

Some of the features are planned for the app are:

<ul>
<li>Individual cloud storage buckets for each teacher to store their resources in</li>
<li>Ability to share lesson plans with relievers</li>
<li>Reuse of lesson plans from year-to-year</li>
<li>Automatic generation of report comment starters based on grades</li>
<li>Tracking of key learning areas to ensure coverage of the curriculum</li>
</ul>

## Further Information

Further information about the project (including link to deployed app), will be added as the project develops.

<strong>The app is a work in progress</strong> and as of 2025 the South Australian Department for Education will be starting to roll-out a South Australian specific curriculum. Access to curriculum documentation is only for teachers at this stage, so this repository will only contain the code for the app, but won't be able to be run as the required curriculum documents are not included.
