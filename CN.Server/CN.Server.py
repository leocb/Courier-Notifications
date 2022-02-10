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


ChannelClientListDict = {}
ClientChannelDict = {}


class WSHandler(tornado.websocket.WebSocketHandler):
    def check_origin(client, origin):
        return True

    def open(client, ChannelID):
        if ChannelID is None or not ChannelID:
            client.write_message("Error: Channel argument is missing")
            print("A client tried to connected without a Channel arg")
            client.close()
            return

        ChannelID = ChannelID.upper()
        ClientChannelDict[client] = ChannelID

        if ChannelID in ChannelClientListDict:
            ChannelClientListDict[ChannelID].append(client)
        else:
            ChannelClientListDict[ChannelID] = [client]

        print("A client connected to " + ChannelID)

    def on_message(client, message):
        try:
            ChannelID = ClientChannelDict[client]
            print(ChannelID + " | " + message)
            EchoList = ChannelClientListDict[ChannelID]
            for c in EchoList:
                c.write_message(message)
        except:
            pass

    def on_close(client):
        try:
            ChannelID = ClientChannelDict[client]
            ChannelClientListDict[ChannelID].remove(client)
            del ClientChannelDict[client]
            print("A client disconnected from " + ChannelID)
        except:
            pass


application = tornado.web.Application(
    [
        (r"/ws/(.*)", WSHandler),
        (r"/", MainHandler),
    ]
)

if __name__ == "__main__":
    print("Ready!")
    application.listen(9091)
    tornado.ioloop.IOLoop.instance().start()
