"""backend URL Configuration"""
import notify.apscheduler
from django.contrib import admin
from django.conf.urls import url, include
from rest_framework import routers
from todo import views as todo_views
from notify import views as notify_views


router = routers.DefaultRouter()
router.register(r'todos', todo_views.TodoView, 'todo')

urlpatterns = [
    url(r'^admin/', admin.site.urls),
    url(r'^api/', include(router.urls)),
    url(r'^notification/$', notify_views.Notification.as_view()),
]
