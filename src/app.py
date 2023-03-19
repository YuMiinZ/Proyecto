from flask import Flask,  render_template, request, redirect, url_for, session # pip install Flask


app = Flask(__name__)

@app.route("/")
def home():
   return render_template("prueba2.html")

