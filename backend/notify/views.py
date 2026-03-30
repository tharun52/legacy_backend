from datetime import datetime, timedelta
from django.http import HttpResponse
from django.views import View
from notify.apscheduler import scheduler
from notify.utils import send_notification
from apscheduler.triggers.date import DateTrigger
from backend import settings


class Notification(View):
    def get(self, request):
        print(request)

        some_delay = 10
        trigger = DateTrigger(
            run_date=datetime.now() + timedelta(seconds=some_delay),
            timezone=settings.TIME_ZONE
        )
        job = scheduler.add_job(
            send_notification, trigger=trigger)
        print(job)

        return HttpResponse({"message": "Notification scheduled!"})
