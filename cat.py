import os
import platform
import random
import string
import subprocess
import threading
import time
import signal
import pip._vendor.requests as requests
import playsound

# Kill the program if a fatal error is encountered
def killProgram():
    os.kill(os.getpid(), signal.SIGKILL)

# Play the song in the background, also loop it
def playCatSound():
    print("Playing cat song!")
    try:
        while True:
            playsound.playsound("./cat.mp3")
    except:
        print("Error playing cat song!")
        killProgram()
    time.sleep(1)

# Start playing the music
threading.Thread(target=playCatSound, daemon=True).start()

# Main function for downloading the cat
def downloadCat():
    print("Obtaining cat image...")
    
    # Gets the cat image from an API
    image = requests.get("https://cataas.com/cat/cute")
    
    # If the API is down, use a backup API
    if image.status_code != 200:
        print("Using backup API!")
        image = requests.get(requests.get("https://api.thecatapi.com/v1/images/search").json()[0]['url'])

    # Write the image to disk
    filename = f"cat{''.join(random.choice(string.ascii_uppercase + string.digits) for _ in range(5))}.png"
    with open(filename, 'wb') as outfile:
        outfile.write(image.content)

    # Function for opening the image, only works on MacOS and Windows since open commands on linux vary by distro
    def openNsomething():
        if platform.system() == "Darwin":
            subprocess.call(('open', '-Wg', filename))
        elif platform.system() == "Windows":
            subprocess.call(('start', '', "/wait", "/min", filename), shell=True)
            time.sleep(1)
        else:
            print("Unsupported OS!")
            killProgram()
            
        os.remove(filename)
        
    # Open the image in a new thread, so that the main thread can start loading the next image by the time the current one is opened.
    threading.Thread(target=openNsomething).start()
    print("Downloaded cat!")

while True:
    downloadCat()
    time.sleep(1)
