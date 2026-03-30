"""
ASGI config for backend project (Channels 1.x).

Exposes the channel layer as a module-level variable named ``channel_layer``.

For more information on this file, see
https://channels.readthedocs.io/en/1.x/deploying.html
"""

import os
from channels.asgi import get_channel_layer

os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'backend.settings')

channel_layer = get_channel_layer()
