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


def playCatSound():
    print("Playing cat song!")
    try:
        while True:
            playsound.playsound("./cat.mp3")
    except:
        print("Error playing cat song!")
        os.kill(os.getpid(), signal.SIGKILL)
    time.sleep(1)

threading.Thread(target=playCatSound, daemon=True).start()

def downloadCat():
    print("Obtaining cat image...")
    
    image = None
    try:
        image = requests.get("https://cataas.com/cat/cute")
    except:
        print("Using backup API!")
        image = requests.get(requests.get("https://api.thecatapi.com/v1/images/search").json()[0]['url'])

    filename = f"cat{''.join(random.choice(string.ascii_uppercase + string.digits) for _ in range(5))}.png"
    with open(filename, 'wb') as outfile:
        outfile.write(image.content)

    def openNsomething():
        if platform.system() == "Darwin":
            subprocess.call(('open', '-Wg', filename))
        elif platform.system() == "Windows":
            subprocess.call(('start /wait', filename))
        else:
            print("Unsupported OS!")
            exit(0)
            
        os.remove(filename)
        
    threading.Thread(target=openNsomething).start()

    print("Downloaded cat!")

while True:
    downloadCat()
    time.sleep(1)