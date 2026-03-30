from channels.routing import route_class
from notify.consumers import ToDoEventConsumer

channel_routing = [
    route_class(ToDoEventConsumer, path=r'^/notifications/$'),
]
