from musicClient import Create
from music import Create as CreateSession

if __name__ == '__main__':
    music = Create()
    Session = CreateSession(10)
    
    while True:
        print(music.GetCommand())
        print(music.state.title)
        command = input("command: ")
        req = music.Execute(command)
        
        ##########################################
        
        if not isinstance(req, dict):
            continue

        if req["state"] is True:
            res = Session.Execute(req["body"])
            music.Execute(res)
        else:
            print(req["body"])