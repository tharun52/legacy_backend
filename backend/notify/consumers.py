import json
from channels.generic.websockets import JsonWebsocketConsumer


class ToDoEventConsumer(JsonWebsocketConsumer):
    channel_session = True

    def connection_groups(self, **kwargs):
        return ['events']

    def connect(self, message, **kwargs):
        print('In connect()')
        self.send({
            'type': 'events.connected',
            'content': {
                'id': 999,
                'title': "CONNECTED",
                'description': "On websocket connected to server",
                'completed': False
            }
        })

    def disconnect(self, message, **kwargs):
        print("Closed websocket with code: {}".format(message))

    def receive(self, content, **kwargs):
        print("Received event: {}".format(content))
        self.send(content)
