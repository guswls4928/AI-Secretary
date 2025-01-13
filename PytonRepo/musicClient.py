from enum import Enum
from pygame import mixer
import json

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
    생성 = ("생성", lambda state : Command.create(state))
    목록 = ("목록", lambda state : Command.list_music(state))
    저장 = ("저장", lambda state : Command.save(state))
    정지 = ("정지", lambda state : Command.pause(state))
    재개 = ("재개", lambda state : Command.resume(state))
    업 = ("업", lambda state : Command.volume_up(state))
    다운 = ("다운", lambda state : Command.volume_down(state))
    상위 = ("상위", lambda state : Command.back(state))
    
    def __init__(self, title, func):
        self.title = title
        self.func = func
    
    def __repr__(self):
        return self.title
    
    def __call__(self, state):
        return self.func(state)
    
    def __eq__(self, value):
        return self.title == value

    @staticmethod
    def create(state):
        return State.생성

    @staticmethod
    def list_music(state):
        return {
            "state" : True,
            "body" : "목록"
        }

    @staticmethod
    def save(state):
        return {
            "state" : True,
            "body" : "저장"
        }

    @staticmethod
    def pause(state):
        mixer.music.pause()
        temp = State.정지
        temp.preState = state.preState
        temp.title = state.title
        return temp

    @staticmethod
    def resume(state):
        mixer.music.unpause()
        temp = State.재생
        temp.preState = state.preState
        temp.title = state.title
        return temp

    @staticmethod
    def volume_up(state):
        mixer.music.set_volume(min(mixer.music.get_volume() + 0.1, 1.0))
        print(f"볼륨: {mixer.music.get_volume():.1f}")
        return state

    @staticmethod
    def volume_down(state):
        mixer.music.set_volume(max(mixer.music.get_volume() - 0.1, 0.0))
        print(f"볼륨: {mixer.music.get_volume():.1f}")
        return state

    @staticmethod
    def back(state):
        mixer.music.stop()
        return state.preState

    @staticmethod
    def set_music(file):
        mixer.music.load(file)

    @staticmethod
    def play():
        mixer.music.play()

class State(Enum):
    기본 = ("음악 생성", [Command.생성, Command.목록])
    생성 = ("생성", ["pop", "jazz", "exciting", "calm", "drum", "guitar", "결정"])
    재생 = ("", [Command.정지, Command.상위])
    정지 = ("", [Command.재개, Command.상위])
    목록 = ("목록", [Command.상위])
    
    def __init__(self, title, commands):
        self.title = title
        self.commands = commands
        self.preState = None
        self.query = []
    
    def __repr__(self):
        return self.name
    
    def __eq__(self, value):
        return self.name == value
    
    def __call__(self, command):
        command = deserialize(command)
        
        if isinstance(deserialize(command), list):
            temp = State.목록
            temp.commands = deserialize(command)
            temp.commands.append(Command.상위)
            temp.preState = self
            return temp
        elif isinstance(deserialize(command), bool):
            return self
        elif isinstance(deserialize(command), dict):
            Command.set_music(open(command["musicPath"], "r"))
            Command.play()
            temp = State.재생
            temp.preState = self
            temp.title = command["musicName"]
            return temp
        
        for cmd in self.commands:
            if command == cmd:
                if self == State.생성:
                    if command != "결정":
                        if command == "pop" or command == "jazz":
                            try:
                                self.query[0] = command
                            except:
                                self.query.append(command)
                        elif command == "exciting" or command == "calm":
                            try:
                                self.query[1] = command
                            except:
                                self.query.append(command)
                        else:
                            self.query.append(command)
                        return {
                            "state" : False,
                            "body" : self.query
                        }
                    else:
                        query = self.query
                        self.query = []
                        return {
                            "state" : True,
                            "body" : serialize(query)
                        }
                try:
                    return cmd(self)
                except:
                    return {
                        "state" : True,
                        "body" : command
                    }
    
    def setCommand(self):
        if self.name == State.재생 and self.name == State.정지:
            if Command.상위 not in self.commands:
                self.commands.append(Command.상위)
            
            if Command.다운 in self.commands:
                self.commands.remove(Command.다운)
            if Command.업 in self.commands:
                self.commands.remove(Command.업)
            
            if mixer.music.get_volume() > 0.0:
                self.commands.append(Command.다운)
            if mixer.music.get_volume() < 1.0:
                self.commands.append(Command.업)
    
    def getCommand(self):
        self.setCommand()
        
        temp =[]
        for cmd in self.commands:
            temp.append(cmd.title if isinstance(cmd, Command) else cmd)
                
        return temp

class Music:
    def __init__(self):
        self.state = State.기본
        self.isSaved = False
        self.commands = []
        
    def Execute(self, command):
        newState = self.state(command)
        self.state = newState if isinstance(newState, State) else self.state
        return newState
    
    def GetCommand(self):
        self.commands = self.state.getCommand()
        return self.commands
    
    def Stop(self):
        mixer.music.stop()
    
def Create():
    mixer.init()
    mixer.music.set_volume(0.5)
    
    return Music()

def GetName():
    return "음악 생성"