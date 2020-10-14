![Build and deploy ASP.Net Core app](https://github.com/bovrhovn/webinar-lambda/workflows/Build%20and%20deploy%20ASP.Net%20Core%20app%20to%20Azure%20Web%20App%20-%20lambada-generator/badge.svg)

# Webinar demo

Repository with demo code to showcase, how to leverage different Azure solutions to work with large amount of data and realtime representations of those results.

## Demo structure

Demo is build across multiple branches:

### main branch (current) is **starter** project, where we have:
- **Generators** - ASP.NET Core project to support CRUD operations - storing and working with Azure Tables - and connection with [IOT Hub](https://azure.microsoft.com/en-us/services/iot-hub/) to manage and control devices
- **Models** project, which contains plain POCO objects to be used in the web app
- **Interface** projects with interfaces and contracts on what to implement
- **Services** projects, which is implementation of that project

### [Azure Search branch](https://github.com/bovrhovn/webinar-lambda/tree/azure-search-functionality), which demonstrate, how easy it is to replace querying Azure Tables with using powerfull search engine with minimal changes

### [Device data generator project](https://github.com/bovrhovn/webinar-lambda/tree/device-data-generate) - adding project with Azure Functions to generate data every 5 mins and store that data to Azure Tables

### CosmosDb integration and realtime visualization project](https://github.com/bovrhovn/webinar-lambda/tree/cosmodb-event-hub) contains code for:
- storing and retrieving data from CosmosDb
- demonstrating creating Azure Functions to react on [CosmosDB change feed](https://docs.microsoft.com/en-us/azure/cosmos-db/change-feed) to send data in realtime to clients, calculate profit and many more operations
- sending reaction on change feed in realtime via [Azure Signalr](https://azure.microsoft.com/en-us/services/signalr-service/) to all connected clients to demonstrate, how we can send information from Azure Function to our own code
- sending email notification via [SendGrid](https://sendgrid.com/)
- subscribing to publisher [EventGrid](https://azure.microsoft.com/en-us/services/event-grid/) and reacting to alert in realtime with the use of Signalr
- reacting on events inside [EventHub](https://azure.microsoft.com/en-us/services/event-hubs/) (messages are sent from Amazon Web Services) and from Kafka with the use of Signalr
- using Azure Search to replace search functionality

### [Toastr integration project](https://github.com/bovrhovn/webinar-lambda/tree/toastplusplus) - leveraging [toastr project](https://github.com/CodeSeven/toastr) to show info on client side

## Scenario

We are building a beer factory management software, which needs to follow these requirements:
1. Have the ability to CRUD factories and to add devices (which can send messages and maintain state). Devices are producing and storing logs on efficiency of producing beer
2. Needs to have the ability to see and notify users about any challenges with that factory and load
3. Needs to have the ability to search through device logs 

![basic structure](https://csacoresettings.blob.core.windows.net/public/beer-factory-v1.png)

Leveraging [Azure](https://azure.com) services, here's the flow implemented in branch [CosmosDb integration and realtime visualization project](https://github.com/bovrhovn/webinar-lambda/tree/cosmodb-event-hub).

![structure with Azure Services](https://csacoresettings.blob.core.windows.net/public/beer-factory-v3.png)

### On thw AWS side (The Outsourcer)

![Slide1](https://user-images.githubusercontent.com/6472374/96035783-30a04a00-0e6c-11eb-85f3-a2a8fb81f26e.PNG)

![Slide2](https://user-images.githubusercontent.com/6472374/96035793-3564fe00-0e6c-11eb-8678-e50061857bdf.PNG)

# Credits

- [Toastr project](https://github.com/CodeSeven/toastr)
- [Font Awesome](https://fontawesome.com/)

