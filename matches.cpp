// Игра спички
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <unistd.h>

#include <stdlib.h>
#include <errno.h>
#include <iostream>
#include <sstream>
#include <vector>
#include <string>
#include <cstring>
#include <pthread.h>
#include <vector>
#include <map>
#include <iterator>

using namespace std;

void *processClient(void *);

void handleError(string msg)
{
    cerr << msg << " error code " << errno << " (" << strerror(errno) << ")\n";
    exit(1);
}
vector<string> split(const string text, const string delims)
{
    vector<string> tokens;
    size_t start = text.find_first_not_of(delims), end = 0;
    while((end = text.find_first_of(delims, start)) != string::npos)
    {
        tokens.push_back(text.substr(start, end - start));
        start = text.find_first_not_of(delims, end);
    }
    if(start != string::npos) tokens.push_back(text.substr(start));
    return tokens;
}
string toString(int a)
{
    stringstream ss;
    ss << a;
    return ss.str();
}

int main(int argc, char* argv[])
{
    int port = 28563;
    srand (time(NULL));
    struct sockaddr_in serverAddress, clientAddress;
    int listenSocket; // РІРїСѓСЃРєР°СЋС‰РёР№ СЃРѕРєРµС‚

//СЃРѕР·РґР°РµРј СЃРѕРєРµС‚ РґР»СЏ РїСЂРёРµРјР° СЃРѕРµРґРёРЅРµРЅРёР№ РІСЃРµС… РєР»РёРµРЅС‚РѕРІ
    listenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
//СЂР°Р·СЂРµС€Р°РµРј РїРѕРІС‚РѕСЂРЅРѕ РёСЃРїРѕР»СЊР·РѕРІР°С‚СЊ С‚РѕС‚ Р¶Рµ РїРѕСЂС‚ СЃРµСЂРІРµСЂСѓ РїРѕСЃР»Рµ РїРµСЂРµР·Р°РїСѓСЃРєР° (РЅСѓР¶РЅРѕ, РµСЃР»Рё СЃРµСЂРІРµСЂ СѓРїР°Р» РІРѕ РІСЂРµРјСЏ РѕС‚РєСЂС‹С‚РѕРіРѕ СЃРѕРµРґРёРЅРµРЅРёСЏ)
    int turnOn = 1;
    if (setsockopt(listenSocket, SOL_SOCKET, SO_REUSEADDR, &turnOn, sizeof(turnOn)) == -1)
        handleError("setsockopt failed:");

// Setup the TCP listening socket
    serverAddress.sin_family = AF_INET;
    serverAddress.sin_addr.s_addr = inet_addr("0.0.0.0");
    serverAddress.sin_port = htons(port);

    if (bind( listenSocket, (sockaddr *) &serverAddress, sizeof(serverAddress)) == -1)
        handleError("bind failed:");

    if (listen(listenSocket, 1000) == -1) handleError("listen failed:");

    while (true)
    {
        int *clientSocket = new int[1];
        *clientSocket = accept(listenSocket, NULL, NULL);
        if (*clientSocket < 0) handleError("accept failed:");
        pthread_t threadId;
        pthread_create(&threadId, NULL, processClient, (void*)clientSocket);
    }
}

static pthread_mutex_t cs_mutex = PTHREAD_RECURSIVE_MUTEX_INITIALIZER_NP;
int concurrentClientCount = 0;
std::map<int, int> games;

bool startsWith(string target, string substring)
{
    size_t found = target.find(substring);

    return found == 0;
}

int createGame()
{
    int gameId = rand() % 1000000001;
    while (games.find(gameId) != games.end()) {
        gameId = rand() % 1000000001;
    }
    games[gameId] = 33;
    return gameId;
}

void removeGame(int gameId) {
    games.erase(gameId);
}

void *processClient(void *dataPtr)
{
    pthread_mutex_lock( &cs_mutex );
    cout << ++concurrentClientCount << " concurrent clients are connected\n";
    pthread_mutex_unlock( &cs_mutex );

    int clientSocket = ((int*)dataPtr)[0];
    delete (int*)dataPtr;

    string recvBuffer(1000, '='); //Р±СѓС„РµСЂ РїСЂРёРµРјР°
    while (true)
    {
        int readBytesCount = 1, err;
        err = recv(clientSocket, &recvBuffer[0], 1, 0);
        while (err > 0 && recvBuffer[readBytesCount-1] != '\r') err = recv(clientSocket, &recvBuffer[readBytesCount++], 1, 0);
        if (err < 0) handleError("recv failed:");
        if (err == 0) break;
        recv(clientSocket, &recvBuffer[readBytesCount++], 1, 0);
        string query = recvBuffer.substr(0,readBytesCount-2);
        cout << "Data received: " << query << "\n";
        string answer;

        if (startsWith(query, "START GAME")) {
            int gameId = createGame();
            answer = "CREATED GAME : " + to_string(gameId) + "\r\n";
        }

        else if (startsWith(query, "MAKE MOVE")) {
            vector<string> args = split(query.substr(10), " ");
            int gameId = stoi(args[0]);
            int matchesCount = stoi(args[1]);

            if (games.find(gameId) != games.end())
            {
                if (matchesCount > 0 && matchesCount < 4)
                {
                    games[gameId] -= matchesCount;
                    if (games[gameId] <= 0 ) {
                        answer = "YOU LOSE! THERE ARE NO MORE MATCHES\r\n";
                        removeGame(gameId);
                    }
                    else {
                        matchesCount = rand() % 3 + 1;
                        games[gameId] -= matchesCount;
                        if (games[gameId] <= 0 ) {
                            answer = "YOU WON! THERE ARE NO MORE MATCHES! SERVER TOOK "
                                     + to_string(matchesCount) + " MATCHES\r\n";
                            removeGame(gameId);
                        } else {
                            answer = "SERVER TOOK " + to_string(matchesCount) +
                                    " MATCHES, THERE ARE LEFT " + to_string(games[gameId]) + " MATCHES\r\n";
                        }
                    }
                } else {
                    answer = "MATCHES COUNT SHOULD BE BETWEEN 1 AND 3\r\n";
                }
            }
            else {
                answer = "CAN'T FIND GAME WITH SUCH ID " + to_string(gameId) + "\r\n";
            }
        }

        send( clientSocket, &answer[0], answer.size(), 0 );
    }
    close(clientSocket);

    pthread_mutex_lock( &cs_mutex );
    concurrentClientCount--;
    pthread_mutex_unlock( &cs_mutex );
}
