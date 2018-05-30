#!/usr/bin/python
# -*- coding: UTF-8 -*-
from twisted.internet import reactor
from autobahn.twisted.websocket import WebSocketServerProtocol, WebSocketServerFactory
import random

port = 28563


class ProcessClient(WebSocketServerProtocol):
    concurrentClientCount = 0
    games = {}

    def onConnect(self, request):
        self.concurrentClientCount += 1
        print("Client connecting: {0}".format(request.peer))
        print(str(self.concurrentClientCount) + " concurrent clients are connected")

    def onClose(self, wasClean, code, reason):
        self.concurrentClientCount -= 1
        print("WebSocket connection closed: {0}".format(reason))

    def onMessage(self, data, isBinary):
        if isBinary:
            print("Can't anwer to binary message")
            return
        data = data.decode('utf8')
        response = ""
        if data.startswith("START GAME"):
            gameId = self.createGame()
            response = "CREATED GAME: " + str(gameId) + "\n"
        elif data.startswith("MAKE MOVE"):
            args = data[10:].split(" ")
            gameId = int(args[0])
            matchesCount = int(args[1])
            if gameId in self.games:
                if matchesCount > 0 and matchesCount < 4:
                    self.games[gameId] -= matchesCount
                    if self.games[gameId] <= 0:
                        response = "YOU LOSE! THERE ARE NO MORE MATCHES"
                        self.removeGame(gameId)
                    else:
                        matchesCount = random.randint(1, 3)
                        self.games[gameId] -= matchesCount
                        if self.games[gameId] <= 0:
                            response = "YOU WON! THERE NO MORE MATCHES! SERVER TOOK " + str(matchesCount) + " MATCHES\n"
                            self.removeGame(gameId)
                        else:
                            response = "SERVER TOOK " + str(matchesCount) + " matches, there are left " + \
                                       str(self.games[gameId]) + " matches\n"
                else:
                    response = "MATCHES COUNT SHOULD BE BETWEEN 1 AND 3\n"
            else:
                response = "CAN'T FIND GAME WITH ID " + str(gameId) + "\n"
        self.sendMessage(response.encode('utf8'), False)
        print("Answer: " + response)

    def createGame(self):
        gameId = random.randint(1, 1000000001)
        while gameId in self.games:
            gameId = random.randint(1, 1000000000)
        self.games[gameId] = 33
        return gameId

    def removeGame(self, gameId):
        self.games.pop(gameId, None)

factory = WebSocketServerFactory(u"ws://0.0.0.0:" + str(port))
factory.protocol = ProcessClient
reactor.listenTCP(port, factory)
reactor.run()
