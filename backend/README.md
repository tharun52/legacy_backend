## Initialization

Create virtual environment and install requirements

```sh
$ python3.6 -m venv env
$ source env/bin/activate
(env) $
(env) $ pip install -r requirements.txt

```

```sh
(env) $ python manage.py migrate
(env) $ python manage.py runserver
```

App will start in brower in [`http://0.0.0.0:8000`](http://127.0.0.1:8000)
