# Stock Hypes Tracking System
The system allows clients to connect to a stream of stock prices of the specified company. Also, the system notifies clients when the any significant stock price change happens and provides the news that potentially could be related to the hype.

## Steps to run locally
 1. Run the application on localhost:7009
 2. Establish web socket connection to the application:
    - use secure websocket connenction:
    ````
        wss://localhost:7009/stockHub
    ````
    - send init connection message:
    ````
        {"protocol":"json","version":1}
    ````
    - send start stream message with the specified symbol and poll interval:
    ````
        {"type":1,"target":"StartStream","arguments":[{"symbol":"AAPL", "interval":5}]}
    ````
    - clients can change symbol and poll interval within current connection by sending update stream message:
    ````
        {"type":1,"target":"UpdateStream","arguments":[{"symbol":"AMZN", "interval":2}]}
    ````

## Infrastructure
The system is implemnted with Akka.NET and Akka Streams. Good location transperency of Akka framework makes it easy to run the same system on a single machine, as well as in the cluster enabling to scale connections and streaming handling separately.

![Infrastructure drawio](https://user-images.githubusercontent.com/25819135/230105015-b0a37c37-7c05-4060-9a00-ca77fb597226.png)

### Actors Model
![Actors drawio](https://user-images.githubusercontent.com/25819135/230104571-bfc921bb-5d73-4755-aacf-32d3c6f218d5.png)
