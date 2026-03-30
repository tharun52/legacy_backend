from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from flask_cors import CORS
import os

app = Flask(__name__)
CORS(app)

app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///todo.db'
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

db = SQLAlchemy(app)

class Todo(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    title = db.Column(db.String(200), nullable=False)
    completed = db.Column(db.Boolean, default=False)

    def to_dict(self):
        return {
            'id': self.id,
            'title': self.title,
            'completed': self.completed
        }

@app.route('/api/todos', methods=['GET'])
def get_todos():
    todos = Todo.query.all()
    return jsonify([t.to_dict() for t in todos])

@app.route('/api/todos', methods=['POST'])
def create_todo():
    data = request.get_json()
    todo = Todo(title=data['title'])
    db.session.add(todo)
    db.session.commit()
    return jsonify(todo.to_dict()), 201

@app.route('/api/todos/<int:id>', methods=['PUT'])
def update_todo(id):
    todo = Todo.query.get_or_404(id)
    data = request.get_json()
    todo.title = data.get('title', todo.title)
    todo.completed = data.get('completed', todo.completed)
    db.session.commit()
    return jsonify(todo.to_dict())

@app.route('/api/todos/<int:id>', methods=['DELETE'])
def delete_todo(id):
    todo = Todo.query.get_or_404(id)
    db.session.delete(todo)
    db.session.commit()
    return jsonify({'message': 'Deleted'})

if __name__ == '__main__':
    db.create_all()
    app.run(host='0.0.0.0', port=5000, debug=True)
