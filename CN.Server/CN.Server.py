import tornado.ioloop
import tornado.web
import tornado.websocket
import tornado.template


class MainHandler(tornado.web.RequestHandler):
    def set_default_headers(self):
        self.set_header("Access-Control-Allow-Origin", "*")

    def get(self):
        loader = tornado.template.Loader(".")
        self.write("Oh hi! What are you doing here? :)")

        
RoomClientListDict = {}
ClientRoomDict = {}

class WSHandler(tornado.websocket.WebSocketHandler):

    def check_origin(client, origin):
        return True

    def open(client, RoomID):
        if RoomID is None:
            client.write_message("room argument is missing")
            client.close();
        else:
            RoomID = RoomID.upper()

            ClientRoomDict[client] = RoomID
            
            if RoomID in RoomClientListDict:
                RoomClientListDict[RoomID].append(client)
            else:
                RoomClientListDict[RoomID] = [client]

            print('client connected to ' + RoomID)

    def on_message(client, message):
        print(ClientRoomDict[client] + '|' + message)
        EchoList = RoomClientListDict[ClientRoomDict[client]]
        for v in EchoList:
            v.write_message(message)

    def on_close(client):
        RoomClientListDict[ClientRoomDict[client]].remove(client)
        del ClientRoomDict[client]
        print('client disconnected')


application = tornado.web.Application([
    (r'/ws/(.*)', WSHandler),
    (r'/', MainHandler),
])

if __name__ == "__main__":
    print('ready!')
    application.listen(9091)
    tornado.ioloop.IOLoop.instance().start()