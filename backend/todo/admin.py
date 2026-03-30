from django.contrib import admin
from todo.models import Todo


class TodoAdmin(admin.ModelAdmin):
    list_display = ('title', 'description', 'completed',
                    'created_at', 'updated_at')


admin.site.register(Todo, TodoAdmin)
