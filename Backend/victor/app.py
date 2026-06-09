from flask import Flask, jsonify
from routes.game_routes import game_bp
from routes.prompts_routes import prompts_bp
from routes.login_routes import login_bp

app = Flask(__name__)
app.json.sort_keys = False

app.register_blueprint(game_bp, url_prefix="/api")
app.register_blueprint(prompts_bp, url_prefix="/api")
app.register_blueprint(login_bp, url_prefix="/api")

@app.route('/', methods=['GET'])
def hello_world():
    return jsonify({'hello': 'world'})

@app.route('/ping', methods=['GET'])
def ping():
    return jsonify({'ping': 'pong'})

@app.route('/pong', methods=['GET'])
def pong():
    return jsonify({'pong': 'ping'})

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', ssl_context="adhoc", port=12002)
