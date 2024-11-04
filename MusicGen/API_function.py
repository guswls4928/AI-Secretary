# import random
# import time

def MusicGen_call(tag): # 입력 tag로 MusicGen 불러오는 함수
		return music

class MusicAssistant:
		def __init__(self):
				self.library = {}
				self.current_music = None # 처음 상태
				self.volume = 5 # [0, 10, 1] # 기본 볼륨 5
				
		def generate_music(self, tag):
			music = MusicGen_call(tag)
		  	self.current_music = {"tag": tag, "music": music} # 음악 생성 하면 current music은 tag와 music
		
		def save_music(self):
				if self.current_music:
					tag = self.current_music["tag" + random.randint(1, 100)]
					if tag not in self.library:
							self.library[tag] = []
					self.library[tag].append(self.current_music["music"])
				else:
					break
						
		def get_saved_music(self, tag):
				if tag in self.library:
						music_list = self.library[tag]
						print(f"'{tag}' 태그로 저장된 음악 목록:")
						for index, music in enumerate(music_list, 1):
								print(f"'{index}.{music}")
				else:
						print(f"'{tag} 태그로 저장된 음악이 없습니다.")
						
		def volume_up(self):
				if self.volume < 10:
						self.volume += 1
				else:
						print("max")
		
		def volume_down(self):
				if self.volume > 0:
						self.volume -= 1
				else:
						print("min")
						
		def pause(self):
				if self.current_music:
						return None
				else:
						break
								

#  	def resume(self):
#				if self.current_music:
#						return 
#				else:
#						break
