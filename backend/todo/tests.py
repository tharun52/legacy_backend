from django.test import TestCase
from todo.models import Todo
from todo.serializers import TodoSerializer

# Test for Todo Model


class TodoModelTest(TestCase):
    def setUp(self):
        Todo.objects.create(title="foo", description="foo", completed=False)
        Todo.objects.create(title="bar", description="bar", completed=False)

    def test_todo(self):
        obj_todo1 = Todo.objects.get(title="foo")
        obj_todo2 = Todo.objects.get(title="bar")

        self.assertEqual(obj_todo1.title, "foo")
        self.assertEqual(obj_todo2.title, "bar")

# Test for Todo Serializer


class TodoSerializerTest(TestCase):
    def setUp(self):
        self.val_dict = {"title": "foo",
                         "description": "bar", "completed": False}

    def test_serializer(self):
        serializer = TodoSerializer(data=self.val_dict)
        if serializer.is_valid():
            self.assertDictEqual(self.val_dict, serializer.data)
        return serializer.errors
