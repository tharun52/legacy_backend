import asyncio
import json
from channels.generic.websocket import JsonWebsocketConsumer
from asgiref.sync import async_to_sync


class ToDoEventConsumer(JsonWebsocketConsumer):
    def connect(self):
        print('In connect()')
        async_to_sync(self.channel_layer.group_add)(
            'events', self.channel_name)

        self.accept()
        self.send_json({
            'type': 'events.connected',
            'content': {
                'id': 999,
                'title': "CONNECTED",
                'description': "On websocket connected to server",
                'completed': False
            }
        })

    def disconnect(self, message):
        print("Closed websocket with code: ", message)
        async_to_sync(self.channel_layer.group_discard)(
            'events',
            self.channel_name
        )
        self.close()

    def receive_json(self, content, **kwargs):
        print("Received event: {}".format(content))
        self.send_json(content)

    def events_todonotify(self, event):
        self.send_json(
            {
                'type': 'events.todonotify',
                'content': event['content']
            }
        )
