from flask import Flask,  render_template, request, redirect, url_for, session # pip install Flask


app = Flask(__name__)

@app.route("/")
def home():
   return render_template("login.html")
   

@app.route("/login", methods=['POST'])
def login():
   email = request.form['email']
   password = request.form['password']

   if 'submit-login' in request.form:
      return render_template('prueba.html')
   elif 'submit-register' in request.form:
      return render_template('prueba3.html')


