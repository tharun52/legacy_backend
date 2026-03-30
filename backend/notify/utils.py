import json
from channels import Group


def send_notification():
    print('Sending notification')
    Group('events').send({
        'text': json.dumps({
            'type': 'events.todonotify',
            'content': {
                'id': 999,
                'title': "ToDo Reminder",
                'description': "wake up early in the morning",
                'completed': True
            }
        })
    })
