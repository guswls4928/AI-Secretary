from enum import Enum
import json, scipy, math, tempfile, os
from transformers import AutoProcessor, MusicgenForConditionalGeneration

dlls_path = os.environ.get("DLLS_PATH", ".")

processor = AutoProcessor.from_pretrained("facebook/musicgen-small")
model = MusicgenForConditionalGeneration.from_pretrained("facebook/musicgen-small")


def generate(query, time):
    inputs = processor(
        text=[query],
        padding=True,
        return_tensors="pt",
    )
    audio_values = model.generate(**inputs, max_new_tokens=min(math.ceil(time / 5) * 256, 1536))


    sampling_rate = model.config.audio_encoder.sampling_rate
    output_dir = tempfile.gettempdir()
    output_filename = os.path.join(output_dir, "musicgen_out.wav")
    scipy.io.wavfile.write(output_filename, rate=sampling_rate, data=audio_values[0, 0].numpy())
    return os.path.abspath(output_filename)

def deserialize(obj):
    if not isinstance(obj, str):
        return obj
    
    if obj[0] in ['[', '{']:
        return json.loads(obj)
    elif obj == "True" or obj == "False":
        return obj == "True"
    
    return obj

def serialize(obj):
    if isinstance(obj, list):
        return json.dumps(obj)
    elif isinstance(obj, dict):
        return json.dumps(obj)
    elif isinstance(obj, bool):
        return str(obj)

class Command(Enum):
    생성 = ("생성", lambda query : Command.Create(query))
    목록 = ("목록", lambda id : Command.MusicList(id))
    저장 = ("저장", lambda id, music : Command.Save(id, music))
    
    def __init__(self, title, func):
        self.title = title
        self.func = func
    
    def __repr__(self):
        return self.title
    
    def __eq__(self, value):
        return self.title == value
    
    
    def __call__(self, query):
        return self.func(query)
    
    def Create(query):
        file = generate(f"{query[1]} {query[0]} track with {query[2:]}", 10)
        
        return {"musicName" : query[0], "musicPath" : file}
    
    def MusicList(id):
        with open(os.path.join(dlls_path, "music/musicList.json"), "r", encoding="utf-8") as f:
            temp = json.load(f)[str(id)]["musicList"]
            return serialize([music['musicName'] for music in temp])
    
    def Save(file):
        return serialize(True)

class Music:
    def __init__(self, id):
        self.id = id
        self.music = None
    
    def Execute(self, *args):
        query = deserialize(args[0])
        if isinstance(query, list):
            self.music = Command.생성(query)
            return serialize(self.music)
        elif query == Command.목록:
            return Command.목록(self.id)
        elif query == Command.저장:
            return Command.저장(self.music)
        else:
            with open(os.path.join(dlls_path, "music/musicList.json"), "r", encoding="utf-8") as f:
                return serialize(next(item for item in json.load(f)[str(self.id)]["musicList"] if item["musicName"] == query))

def Create(id):
    return Music(id)

def GetName():
    return "음악 생성"